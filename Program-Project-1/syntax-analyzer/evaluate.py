from calc_yacc import parser

def eval_ast(node):
    t = node[0]
    if t == 'num':
        return node[1]
    if t == 'neg':
        return -eval_ast(node[1])
    if t == '+':
        return eval_ast(node[1]) + eval_ast(node[2])
    if t == '-':
        return eval_ast(node[1]) - eval_ast(node[2])
    if t == '*':
        return eval_ast(node[1]) * eval_ast(node[2])
    if t == '/':
        return eval_ast(node[1]) / eval_ast(node[2])
    raise ValueError(f'Unknown node: {t}')

def main(expr: str):
    ast = parser.parse(expr)
    print("AST:", ast)
    print("Result:", eval_ast(ast))

if __name__ == '__main__':
    try:
        expr = input("expr> ")
        main(expr)
    except KeyboardInterrupt:
        pass
