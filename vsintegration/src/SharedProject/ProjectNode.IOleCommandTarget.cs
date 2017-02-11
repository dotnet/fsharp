/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
//#define CCI_TRACING
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Contains the IOleCommandTarget implementation for ProjectNode
    /// </summary>
    internal abstract partial class ProjectNode {
        #region initiation of command execution

        /// <summary>
        /// Executes a command that can only be executed once the whole selection is known.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmdId">The command to be executed.</param>
        /// <param name="cmdExecOpt">Values describe how the object should execute the command.</param>
        /// <param name="vaIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="vaOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <param name="commandOrigin">The origin of the command. From IOleCommandTarget or hierarchy.</param>
        /// <param name="selectedNodes">The list of the selected nodes.</param>
        /// <param name="handled">An out parameter specifying that the command was handled.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        protected virtual int ExecCommandThatDependsOnSelectedNodes(Guid cmdGroup, uint cmdId, uint cmdExecOpt, IntPtr vaIn, IntPtr vaOut, CommandOrigin commandOrigin, IList<HierarchyNode> selectedNodes, out bool handled) {
            handled = false;
            if (cmdGroup == VsMenus.guidVsUIHierarchyWindowCmds) {
                switch (cmdId) {
                    case (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_RightClick:
                        // The UIHWCMDID_RightClick is what tells an IVsUIHierarchy in a UIHierarchyWindow 
                        // to put up the context menu.  Since the mouse may have moved between the 
                        // mouse down and the mouse up, GetCursorPos won't tell you the right place 
                        // to put the context menu (especially if it came through the keyboard).  
                        // So we pack the proper menu position into pvaIn by
                        // memcpy'ing a POINTS struct into the VT_UI4 part of the pvaIn variant.  The
                        // code to unpack it looks like this:
                        //			ULONG ulPts = V_UI4(pvaIn);
                        //			POINTS pts;
                        //			memcpy((void*)&pts, &ulPts, sizeof(POINTS));
                        // You then pass that POINTS into DisplayContextMenu.
                        handled = true;
                        return this.DisplayContextMenu(selectedNodes, vaIn);
                    default:
                        break;
                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                switch ((VsCommands2K)cmdId) {
                    case VsCommands2K.ViewInClassDiagram:
                        handled = true;
                        return this.ShowInDesigner(selectedNodes);
                }
            }

            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// Executes command that are independent of a selection.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmdId">The command to be executed.</param>
        /// <param name="cmdExecOpt">Values describe how the object should execute the command.</param>
        /// <param name="vaIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="vaOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <param name="commandOrigin">The origin of the command. From IOleCommandTarget or hierarchy.</param>
        /// <param name="handled">An out parameter specifying that the command was handled.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        protected virtual int ExecCommandIndependentOfSelection(Guid cmdGroup, uint cmdId, uint cmdExecOpt, IntPtr vaIn, IntPtr vaOut, CommandOrigin commandOrigin, out bool handled) {
            handled = false;

            if (IsClosed) {
                return VSConstants.E_FAIL;
            }

            if (cmdGroup == VsMenus.guidStandardCommandSet97) {
                if (commandOrigin == CommandOrigin.OleCommandTarget) {
                    switch ((VsCommands)cmdId) {
                        case VsCommands.Cut:
                        case VsCommands.Copy:
                        case VsCommands.Paste:
                        case VsCommands.Rename:
                            handled = true;
                            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
                    }
                }

                switch ((VsCommands)cmdId) {
                    case VsCommands.Copy:
                        handled = true;
                        return this.CopyToClipboard();

                    case VsCommands.Cut:
                        handled = true;
                        return this.CutToClipboard();

                    case VsCommands.SolutionCfg:
                        handled = true;
                        return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

                    case VsCommands.SearchCombo:
                        handled = true;
                        return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                // There should only be the project node who handles these and should manifest in the same action regardles of selection.
                switch ((VsCommands2K)cmdId) {
                    case VsCommands2K.SHOWALLFILES:
                        handled = true;
                        return this.ShowAllFiles();
                    case VsCommands2K.ADDREFERENCE:
                        handled = true;
                        return this.AddProjectReference();
                }
            }

            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// The main entry point for command excection. Gets called from the IVsUIHierarchy and IOleCommandTarget methods.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmdId">The command to be executed.</param>
        /// <param name="cmdExecOpt">Values describe how the object should execute the command.</param>
        /// <param name="vaIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="vaOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <param name="commandOrigin">The origin of the command. From IOleCommandTarget or hierarchy.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        protected virtual int InternalExecCommand(Guid cmdGroup, uint cmdId, uint cmdExecOpt, IntPtr vaIn, IntPtr vaOut, CommandOrigin commandOrigin) {
            if (IsClosed) {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            if (cmdGroup == Guid.Empty) {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            if ((cmdExecOpt & 0xFFFF) == (uint)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP &&
                (cmdExecOpt >> 16) == (uint)VsMenus.VSCmdOptQueryParameterList &&
                vaOut != IntPtr.Zero) {
                // The command accepts arguments and VS is trying to get the
                // format string before executing the actual command.
                // Format strings are space-separated lists of switches:
                //
                //   s,s1,switch1 w,s2,switch2 ...
                var args = QueryCommandArguments(cmdGroup, cmdId, commandOrigin);
                if (string.IsNullOrEmpty(args)) {
                    return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
                }
                Marshal.GetNativeVariantForObject(args, vaOut);
                return VSConstants.S_OK;
            }

            IList<HierarchyNode> selectedNodes = GetSelectedNodes();

            // Check if all nodes can execute a command. If there is at least one that cannot return not handled.
            foreach (HierarchyNode node in selectedNodes) {
                if (!node.CanExecuteCommand) {
                    return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
                }
            }

            // Handle commands that are independent of a selection.
            bool handled = false;
            int returnValue = this.ExecCommandIndependentOfSelection(cmdGroup, cmdId, cmdExecOpt, vaIn, vaOut, commandOrigin, out handled);
            if (handled) {
                return returnValue;
            }


            // Now handle commands that need the selected nodes as input parameter.
            returnValue = this.ExecCommandThatDependsOnSelectedNodes(cmdGroup, cmdId, cmdExecOpt, vaIn, vaOut, commandOrigin, selectedNodes, out handled);
            if (handled) {
                return returnValue;
            }

            returnValue = (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

            // Handle commands iteratively. The same action will be executed for all of the selected items.
            foreach (HierarchyNode node in selectedNodes) {
                try {
                    returnValue = node.ExecCommandOnNode(cmdGroup, cmdId, cmdExecOpt, vaIn, vaOut);
                } catch (COMException e) {
                    Trace.WriteLine("Exception : " + e.Message);
                    returnValue = e.ErrorCode;
                }
                if (returnValue != VSConstants.S_OK) {
                    break;
                }
            }

            if (returnValue == VSConstants.E_ABORT || returnValue == VSConstants.OLE_E_PROMPTSAVECANCELLED) {
                returnValue = VSConstants.S_OK;
            }

            return returnValue;
        }

        /// <summary>
        /// Returns a space-separated string representing the supported switches
        /// for the specified command.
        /// </summary>
        protected internal virtual string QueryCommandArguments(Guid cmdGroup, uint cmdId, CommandOrigin commandOrigin) {
            return null;
        }

        #endregion

        #region query command handling

        /// <summary>
        /// Handles menus originating from IOleCommandTarget.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="handled">Specifies whether the menu was handled.</param>
        /// <returns>A QueryStatusResult describing the status of the menu.</returns>
        protected virtual QueryStatusResult QueryStatusCommandFromOleCommandTarget(Guid cmdGroup, uint cmd, out bool handled) {
            handled = false;
            // NOTE: We only want to support Cut/Copy/Paste/Delete/Rename commands
            // if focus is in the project window. This means that we should only
            // support these commands if they are dispatched via IVsUIHierarchy
            // interface and not if they are dispatch through IOleCommandTarget
            // during the command routing to the active project/hierarchy.
            if (VsMenus.guidStandardCommandSet97 == cmdGroup) {

                switch ((VsCommands)cmd) {
                    case VsCommands.Copy:
                    case VsCommands.Paste:
                    case VsCommands.Cut:
                    case VsCommands.Rename:
                        handled = true;
                        return QueryStatusResult.NOTSUPPORTED;
                }
            }
                // The reference menu and the web reference menu should always be shown.
            else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                switch ((VsCommands2K)cmd) {
                    case VsCommands2K.ADDREFERENCE:
                        handled = true;
                        if (GetReferenceContainer() != null) {
                            return QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        } else {
                            return QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                        }
                }
            }
            return QueryStatusResult.NOTSUPPORTED;
        }

        /// <summary>
        /// Specifies which command does not support multiple selection and should be disabled if multi-selected.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="selectedNodes">The list of selected nodes.</param>
        /// <param name="handled">Specifies whether the menu was handled.</param>
        /// <returns>A QueryStatusResult describing the status of the menu.</returns>
        protected virtual QueryStatusResult DisableCommandOnNodesThatDoNotSupportMultiSelection(Guid cmdGroup, uint cmd, IList<HierarchyNode> selectedNodes, out bool handled) {
            handled = false;
            QueryStatusResult queryResult = QueryStatusResult.NOTSUPPORTED;
            if (selectedNodes == null || selectedNodes.Count == 1) {
                return queryResult;
            }

            if (VsMenus.guidStandardCommandSet97 == cmdGroup) {
                switch ((VsCommands)cmd) {
                    case VsCommands.Cut:
                    case VsCommands.Copy:
                        // If the project node is selected then cut and copy is not supported.
                        if (selectedNodes.Contains(this)) {
                            queryResult = QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                            handled = true;
                        }
                        break;

                    case VsCommands.Paste:
                    case VsCommands.NewFolder:
                        queryResult = QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                        handled = true;
                        break;
                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                switch ((VsCommands2K)cmd) {
                    case VsCommands2K.QUICKOBJECTSEARCH:
                    case VsCommands2K.SETASSTARTPAGE:
                    case VsCommands2K.ViewInClassDiagram:
                        queryResult = QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                        handled = true;
                        break;
                }
            }

            return queryResult;
        }

        /// <summary>
        /// Disables commands when the project is in run/break mode.
        /// </summary>/
        /// <param name="commandGroup">Unique identifier of the command group</param>
        /// <param name="command">The command to be executed.</param>
        /// <returns>A QueryStatusResult describing the status of the menu.</returns>
        protected virtual bool DisableCmdInCurrentMode(Guid commandGroup, uint command) {
            if (IsClosed) {
                return false;
            }

            // Don't ask if it is not these commandgroups.
            if (commandGroup == VsMenus.guidStandardCommandSet97 ||
                commandGroup == VsMenus.guidStandardCommandSet2K ||
                commandGroup == SharedCommandGuid) {
                if (this.IsCurrentStateASuppressCommandsMode()) {
                    if (commandGroup == VsMenus.guidStandardCommandSet97) {
                        switch ((VsCommands)command) {
                            default:
                                break;
                            case VsCommands.AddExistingItem:
                            case VsCommands.AddNewItem:
                            case VsCommands.NewFolder:
                            case VsCommands.Remove:
                            case VsCommands.Cut:
                            case VsCommands.Paste:
                            case VsCommands.Copy:
                            case VsCommands.EditLabel:
                            case VsCommands.Rename:
                            case VsCommands.UnloadProject:
                                return true;
                        }
                    } else if (commandGroup == VsMenus.guidStandardCommandSet2K) {
                        switch ((VsCommands2K)command) {
                            default:
                                break;
                            case VsCommands2K.EXCLUDEFROMPROJECT:
                            case VsCommands2K.INCLUDEINPROJECT:
                            case VsCommands2K.ADDWEBREFERENCECTX:
                            case VsCommands2K.ADDWEBREFERENCE:
                            case VsCommands2K.ADDREFERENCE:
                            case VsCommands2K.SETASSTARTPAGE:
                                return true;
                        }
                    } else if (commandGroup == SharedCommandGuid) {
                        switch ((SharedCommands)command) {
                            case SharedCommands.AddExistingFolder:
                                return true;
                        }
                    }
                }
                // If we are not in a cut or copy mode then disable the paste command
                else if (commandGroup == VsMenus.guidStandardCommandSet97 && (VsCommands)command == VsCommands.Paste) {
                    return !this.AllowPasteCommand();
                }
            }

            return false;
        }


        /// <summary>
        /// Queries the object for the command status on a list of selected nodes.
        /// </summary>
        /// <param name="cmdGroup">A unique identifier of the command group.</param>
        /// <param name="cCmds">The number of commands in the prgCmds array</param>
        /// <param name="prgCmds">A caller-allocated array of OLECMD structures that indicate the commands for which the caller requires status information. This method fills the cmdf member of each structure with values taken from the OLECMDF enumeration</param>
        /// <param name="pCmdText">Pointer to an OLECMDTEXT structure in which to return the name and/or status information of a single command. Can be NULL to indicate that the caller does not require this information. </param>
        /// <param name="commandOrigin">Specifies the origin of the command. Either it was called from the QueryStatusCommand on IVsUIHierarchy or from the IOleCommandTarget</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        protected virtual int QueryStatusSelection(Guid cmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText, CommandOrigin commandOrigin) {
            if (IsClosed) {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            if (cmdGroup == Guid.Empty) {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }

            Utilities.ArgumentNotNull("prgCmds", prgCmds);

            uint cmd = prgCmds[0].cmdID;
            QueryStatusResult queryResult = QueryStatusResult.NOTSUPPORTED;

            // For now ask this node (that is the project node) to disable or enable a node.
            // This is an optimization. Why should we ask each node for its current state? They all are in the same state.
            // Also please note that we return QueryStatusResult.INVISIBLE instead of just QueryStatusResult.SUPPORTED.
            // The reason is that if the project has nested projects, then providing just QueryStatusResult.SUPPORTED is not enough.
            // What will happen is that the nested project will show grayed commands that belong to this project and does not belong to the nested project. (like special commands implemented by subclassed projects).
            // The reason is that a special command comes in that is not handled because we are in debug mode. Then VsCore asks the nested project can you handle it.
            // The nested project does not know about it, thus it shows it on the nested project as grayed.
            if (this.DisableCmdInCurrentMode(cmdGroup, cmd)) {
                queryResult = QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
            } else {
                bool handled = false;

                if (commandOrigin == CommandOrigin.OleCommandTarget) {
                    queryResult = this.QueryStatusCommandFromOleCommandTarget(cmdGroup, cmd, out handled);
                }

                if (!handled) {
                    IList<HierarchyNode> selectedNodes = GetSelectedNodes();

                    // Want to disable in multiselect case.
                    if (selectedNodes != null && selectedNodes.Count > 1) {
                        queryResult = this.DisableCommandOnNodesThatDoNotSupportMultiSelection(cmdGroup, cmd, selectedNodes, out handled);
                    }

                    // Now go and do the job on the nodes.
                    if (!handled) {
                        queryResult = this.QueryStatusSelectionOnNodes(selectedNodes, cmdGroup, cmd, pCmdText);
                    }

                }
            }

            // Process the results set in the QueryStatusResult
            if (queryResult != QueryStatusResult.NOTSUPPORTED) {
                // Set initial value
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;

                if ((queryResult & QueryStatusResult.ENABLED) != 0) {
                    prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;
                }

                if ((queryResult & QueryStatusResult.INVISIBLE) != 0) {
                    prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_INVISIBLE;
                }

                if ((queryResult & QueryStatusResult.LATCHED) != 0) {
                    prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_LATCHED;
                }

                return VSConstants.S_OK;
            }

            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// Queries the selected nodes for the command status. 
        /// A command is supported iff any nodes supports it.
        /// A command is enabled iff all nodes enable it.
        /// A command is invisible iff any node sets invisibility.
        /// A command is latched only if all are latched.
        /// </summary>
        /// <param name="selectedNodes">The list of selected nodes.</param>
        /// <param name="cmdGroup">A unique identifier of the command group.</param>
        /// <param name="cmd">The command id to query for.</param>
        /// <param name="pCmdText">Pointer to an OLECMDTEXT structure in which to return the name and/or status information of a single command. Can be NULL to indicate that the caller does not require this information. </param>
        /// <returns>Retuns the result of the query on the slected nodes.</returns>
        protected virtual QueryStatusResult QueryStatusSelectionOnNodes(IList<HierarchyNode> selectedNodes, Guid cmdGroup, uint cmd, IntPtr pCmdText) {
            if (selectedNodes == null || selectedNodes.Count == 0) {
                return QueryStatusResult.NOTSUPPORTED;
            }

            int result = 0;
            bool supported = false;
            bool enabled = true;
            bool invisible = false;
            bool latched = true;
            QueryStatusResult tempQueryResult = QueryStatusResult.NOTSUPPORTED;

            foreach (HierarchyNode node in selectedNodes) {
                result = node.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref tempQueryResult);
                if (result < 0) {
                    break;
                }

                // cmd is supported iff any node supports cmd
                // cmd is enabled iff all nodes enable cmd
                // cmd is invisible iff any node sets invisibility
                // cmd is latched only if all are latched.
                supported = supported || ((tempQueryResult & QueryStatusResult.SUPPORTED) != 0);
                enabled = enabled && ((tempQueryResult & QueryStatusResult.ENABLED) != 0);
                invisible = invisible || ((tempQueryResult & QueryStatusResult.INVISIBLE) != 0);
                latched = latched && ((tempQueryResult & QueryStatusResult.LATCHED) != 0);
            }

            QueryStatusResult queryResult = QueryStatusResult.NOTSUPPORTED;

            if (result >= 0 && supported) {
                queryResult = QueryStatusResult.SUPPORTED;

                if (enabled) {
                    queryResult |= QueryStatusResult.ENABLED;
                }

                if (invisible) {
                    queryResult |= QueryStatusResult.INVISIBLE;
                }

                if (latched) {
                    queryResult |= QueryStatusResult.LATCHED;
                }
            }

            return queryResult;
        }

        #endregion

        /// <summary>
        /// CommandTarget.Exec is called for most major operations if they are NOT UI based. Otherwise IVSUInode::exec is called first
        /// </summary>
        public virtual int Exec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdExecOpt, IntPtr pvaIn, IntPtr pvaOut) {
            return this.InternalExecCommand(guidCmdGroup, nCmdId, nCmdExecOpt, pvaIn, pvaOut, CommandOrigin.OleCommandTarget);
        }

        /// <summary>
        /// Queries the object for the command status
        /// </summary>
        /// <remarks>we only support one command at a time, i.e. the first member in the OLECMD array</remarks>
        public virtual int QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            return this.QueryStatusSelection(guidCmdGroup, cCmds, prgCmds, pCmdText, CommandOrigin.OleCommandTarget);
        }
    }
}
