import pytest
from calc_yacc import parser
from evaluate import eval_ast


def parse(text):
    return parser.parse(text)


def test_eval_number():
    assert eval_ast(("num", 5)) == 5


def test_eval_unary_minus():
    assert eval_ast(("neg", ("num", 5))) == -5


def test_eval_addition():
    assert eval_ast(parse("3+2")) == 5


def test_eval_precedence():
    assert eval_ast(parse("3+2*4")) == 11


def test_eval_parentheses():
    assert eval_ast(parse("(3+2)*4")) == 20


def test_eval_division():
    assert eval_ast(parse("6/3")) == 2


@pytest.mark.parametrize(
    "expr, expected",
    [
        ("1+2", 3),
        ("7-4", 3),
        ("2*5", 10),
        ("8/2", 4),
        ("-(3+2)", -5),
    ],
)
def test_eval_parameterized(expr, expected):
    assert eval_ast(parse(expr)) == expected


def test_eval_unknown_node_raises():
    with pytest.raises(ValueError, match="Unknown node"):
        eval_ast(("bad_node", 1))