import ply.yacc as yacc
from calc_lex import tokens

precedence = (
    ('left', 'PLUS', 'MINUS'),
    ('left', 'TIMES', 'DIVIDE'),
    ('right', 'UMINUS'),
)


def p_expr_binop(p):
    '''expr : expr PLUS expr
            | expr MINUS expr
            | expr TIMES expr
            | expr DIVIDE expr'''
    p[0] = (p[2], p[1], p[3])


def p_expr_group(p):
    'expr : LPAREN expr RPAREN'
    p[0] = p[2]


def p_expr_number(p):
    'expr : NUMBER'
    p[0] = ('num', p[1])


def p_expr_uminus(p):
    'expr : MINUS expr %prec UMINUS'
    p[0] = ('neg', p[2])


def p_error(p):
    if p:
        raise SyntaxError(f"Syntax error at token {p.type}, value {p.value}")
    raise SyntaxError("Syntax error at EOF")


parser = yacc.yacc()