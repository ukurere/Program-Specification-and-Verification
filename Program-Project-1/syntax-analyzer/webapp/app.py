import os, sys
from flask import Flask, request, jsonify, render_template

# Додаємо шлях до кореня проєкту, щоб бачити calc_yacc.py
sys.path.append(os.path.dirname(os.path.dirname(__file__)))
from calc_yacc import parser

app = Flask(__name__)

def ast_to_dict(node):
    """('+, ('num',3), ('num',4)) -> {name:'+', children:[...]}"""
    name = str(node[1]) if node[0] == 'num' else node[0]
    d = {"name": name, "children": []}
    for child in node[1:]:
        if isinstance(child, tuple):
            d["children"].append(ast_to_dict(child))
    return d

@app.route("/")
def index():
    return render_template("index.html")

@app.route("/api/parse", methods=["POST"])
def api_parse():
    expr = (request.get_json(force=True) or {}).get("expr", "")
    try:
        ast = parser.parse(expr)
        if ast is None:
            return jsonify({"ok": False, "error": "Parse error"}), 400
        return jsonify({"ok": True, "tree": ast_to_dict(ast)})
    except Exception as e:
        return jsonify({"ok": False, "error": str(e)}), 400

if __name__ == "__main__":
    app.run(debug=True)
