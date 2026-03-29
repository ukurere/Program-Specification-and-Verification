import os
import sys

PROJECT_ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
if PROJECT_ROOT not in sys.path:
    sys.path.insert(0, PROJECT_ROOT)

import pytest
from calc_lex import lexer
from calc_yacc import parser


@pytest.fixture
def lexer_instance():
    lexer.lineno = 1
    return lexer


@pytest.fixture
def parser_instance():
    return parser