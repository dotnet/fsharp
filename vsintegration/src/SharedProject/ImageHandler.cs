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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.VisualStudioTools.Project {
    public class ImageHandler : IDisposable {
        private ImageList imageList;
        private List<IntPtr> iconHandles;
        private static volatile object Mutex;
        private bool isDisposed;

        /// <summary>
        /// Initializes the <see cref="RDTListener"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ImageHandler() {
            Mutex = new object();
        }

        /// <summary>
        /// Builds an empty ImageHandler object.
        /// </summary>
        public ImageHandler() {
        }

        /// <summary>
        /// Builds an ImageHandler object from a Stream providing the bitmap that
        /// stores the images for the image list.
        /// </summary>
        public ImageHandler(Stream resourceStream) {
            Utilities.ArgumentNotNull("resourceStream", resourceStream);
            imageList = Utilities.GetImageList(resourceStream);
        }

        /// <summary>
        /// Builds an ImageHandler object from an ImageList object.
        /// </summary>
        public ImageHandler(ImageList list) {
            Utilities.ArgumentNotNull("list", list);

            imageList = list;
        }

        /// <summary>
        /// Closes the ImageHandler object freeing its resources.
        /// </summary>
        public void Close() {
            if (null != iconHandles) {
                foreach (IntPtr hnd in iconHandles) {
                    if (hnd != IntPtr.Zero) {
                        NativeMethods.DestroyIcon(hnd);
                    }
                }
                iconHandles = null;
            }

            if (null != imageList) {
                imageList.Dispose();
                imageList = null;
            }
        }

        /// <summary>
        /// Add an image to the ImageHandler.
        /// </summary>
        /// <param name="image">the image object to be added.</param>
        public void AddImage(Image image) {
            Utilities.ArgumentNotNull("image", image);
            if (null == imageList) {
                imageList = new ImageList();
            }
            imageList.Images.Add(image);
            if (null != iconHandles) {
                iconHandles.Add(IntPtr.Zero);
            }
        }

        /// <summary>
        /// Get or set the ImageList object for this ImageHandler.
        /// </summary>
        public ImageList ImageList {
            get { return imageList; }
            set {
                Close();
                imageList = value;
            }
        }

        /// <summary>
        /// Returns the handle to an icon build from the image of index
        /// iconIndex in the image list.
        /// </summary>
        public IntPtr GetIconHandle(int iconIndex) {
            Utilities.CheckNotNull(imageList);
            // Make sure that the list of handles is initialized.
            if (null == iconHandles) {
                InitHandlesList();
            }

            // Verify that the index is inside the expected range.
            if ((iconIndex < 0) || (iconIndex >= iconHandles.Count)) {
                throw new ArgumentOutOfRangeException("iconIndex");
            }

            // Check if the icon is in the cache.
            if (IntPtr.Zero == iconHandles[iconIndex]) {
                Bitmap bitmap = imageList.Images[iconIndex] as Bitmap;
                // If the image is not a bitmap, then we can not build the icon,
                // so we have to return a null handle.
                if (null == bitmap) {
                    return IntPtr.Zero;
                }

                iconHandles[iconIndex] = bitmap.GetHicon();
            }

            return iconHandles[iconIndex];
        }

        private void InitHandlesList() {
            iconHandles = new List<IntPtr>(imageList.Images.Count);
            for (int i = 0; i < imageList.Images.Count; ++i) {
                iconHandles.Add(IntPtr.Zero);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private void Dispose(bool disposing) {
            if (!this.isDisposed) {
                lock (Mutex) {
                    if (disposing) {
                        this.imageList.Dispose();
                    }

                    this.isDisposed = true;
                }
            }
        }
    }
}
