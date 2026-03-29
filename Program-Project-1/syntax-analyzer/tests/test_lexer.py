import pytest
from calc_lex import lexer


def tokenize(text):
    lexer.input(text)
    result = []
    while True:
        tok = lexer.token()
        if tok is None:
            break
        result.append((tok.type, tok.value))
    return result


def test_tokenize_simple_expression():
    tokens = tokenize("3 + 4 * 2")
    assert tokens == [
        ("NUMBER", 3),
        ("PLUS", "+"),
        ("NUMBER", 4),
        ("TIMES", "*"),
        ("NUMBER", 2),
    ]


def test_tokenize_parentheses():
    tokens = tokenize("(2+5)")
    assert tokens == [
        ("LPAREN", "("),
        ("NUMBER", 2),
        ("PLUS", "+"),
        ("NUMBER", 5),
        ("RPAREN", ")"),
    ]


def test_tokenize_ignores_spaces_and_tabs():
    tokens = tokenize(" \t 12 \t +\t 7 ")
    assert tokens == [
        ("NUMBER", 12),
        ("PLUS", "+"),
        ("NUMBER", 7),
    ]


def test_illegal_character_raises_value_error():
    with pytest.raises(ValueError, match="Illegal character"):
        tokenize("2 & 3")