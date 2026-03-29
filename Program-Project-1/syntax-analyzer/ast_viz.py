from graphviz import Digraph
from calc_yacc import parser

def draw_ast(node, dot=None, parent=None):
    if dot is None:
        dot = Digraph(node_attr={'shape':'circle'})
    nid = str(id(node))
    label = str(node[1]) if node[0]=='num' else node[0]
    dot.node(nid, label)
    if parent:
        dot.edge(parent, nid)
    for child in node[1:]:
        if isinstance(child, tuple):
            draw_ast(child, dot, nid)
    return dot

if __name__ == '__main__':
    expr = input("expr for AST> ")
    ast = parser.parse(expr)
    dot = draw_ast(ast)
    dot.render('ast', format='png', cleanup=True)
    print("Saved ast.png")
