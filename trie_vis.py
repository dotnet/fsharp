'''
    A small program to visualize a trie data structure in the form of decision tree graph.
'''
#imports
import json
import pydot

# function to return a new dict template
def struct():
    struct = {
        
    }
    return struct

# getting list of words as input from the file
file_ = open('./input.txt', 'r')
file_text = file_.read()
file_len = len(file_text)
file_.close()

# trie making stuff happening (hard to explain)
tmp_s = struct()
root = tmp_s
for line in file_text.split('\n'):
    for c in line.split('.'):
        if c not in tmp_s:
            tmp_s[c] = struct()
        tmp_s = tmp_s[c]
    tmp_s = root
    cur_word = []

# saving the trie in a json file
with open('output.json', 'w') as fp:
    json.dump(root, fp, indent=4)

# converting and saving the trie to dot language decision tree graph using pydot
# // code taken and modified from stackoverflow (https://stackoverflow.com/questions/13688410/dictionary-object-to-decision-tree-in-pydot)
rt = {'': root}
counter = 0
def draw(parent_name, child_name):
    global counter
    counter += 1
    p_n = parent_name
    c_n = child_name
    graph.add_node(pydot.Node(p_n, label=parent_name.split('_')[0]))
    graph.add_node(pydot.Node(c_n, label=child_name.split('_')[0]))
    edge = pydot.Edge(p_n, c_n)
    graph.add_edge(edge)

def visit(node, parent=None):
    global counter
    for k,v in node.items():
        if isinstance(v, dict):
            # We start with the root node whose parent is None
            # we don't want to graph the None node
            k = k + '_' + str(counter)
            if parent:
                draw(parent, k)
            visit(v, k)
        else:
            # drawing the label using a distinct name
            v = v + '_' + str(counter)
            draw(parent, v)

graph = pydot.Dot(graph_type='digraph')
visit(rt)
graph.write_pdf('output.pdf')

# end of program