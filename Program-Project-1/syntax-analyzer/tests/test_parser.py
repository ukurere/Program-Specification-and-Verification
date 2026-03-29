import pytest
from calc_yacc import parser


def parse(text):
    return parser.parse(text)


def test_parse_number():
    assert parse("42") == ("num", 42)


def test_parse_addition():
    assert parse("3+2") == ("+", ("num", 3), ("num", 2))


def test_parse_precedence():
    assert parse("3+2*4") == (
        "+",
        ("num", 3),
        ("*", ("num", 2), ("num", 4)),
    )


def test_parse_parentheses_override_precedence():
    assert parse("(3+2)*4") == (
        "*",
        ("+", ("num", 3), ("num", 2)),
        ("num", 4),
    )


def test_parse_unary_minus():
    assert parse("-5") == ("neg", ("num", 5))


@pytest.mark.parametrize(
    "expr, expected",
    [
        ("1+2", ("+", ("num", 1), ("num", 2))),
        ("7-4", ("-", ("num", 7), ("num", 4))),
        ("6/3", ("/", ("num", 6), ("num", 3))),
    ],
)
def test_parse_parameterized(expr, expected):
    assert parse(expr) == expected


def test_syntax_error_raises():
    with pytest.raises(SyntaxError, match="Syntax error"):
        parse("3+")