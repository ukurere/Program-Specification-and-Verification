# Програмний проєкт 4 — Статичний аналіз коду

**Проєкт:** ExpressionAnalyzer (C#, .NET 9)  
**Аналізатор:** SonarAnalyzer.CSharp 10.26.0 (Roslyn-аналізатор)  
**Метрики:** Microsoft.CodeAnalysis.Metrics  

---

## 1. Виявлені проблеми (SonarAnalyzer)

| Файл | Рядок | Правило | Опис |
|------|-------|---------|------|
| `Evaluator.cs` | 22 | **S1244** | Do not check floating point equality with exact values, use a range instead |

### Опис правила S1244

**Назва:** Floating point numbers should not be tested for equality  
**Категорія:** Bug / Reliability  
**Серйозність:** Major  

**Проблема:** Числа з плаваючою комою (float/double) зберігаються у форматі IEEE 754 і можуть мати похибку округлення. Перевірка `x == 0.0` або `x == y` може дати хибний результат навіть якщо числа математично рівні.

**Приклад порушення:**
```csharp
double divisor = Evaluate(node.Right!);
if (divisor == 0)          // S1244: небезпечне порівняння
    throw new DivideByZeroException(...);
```

**Виправлення — використовувати epsilon-порівняння:**
```csharp
if (Math.Abs(divisor) < 1e-10)   // безпечне порівняння з допуском
    throw new DivideByZeroException(...);
```

---

## 2. Метрики коду (до рефакторингу)

### Зведена таблиця по класах

| Клас / Метод | Cyclomatic Complexity | Maintainability Index | Source Lines |
|---|---|---|---|
| **Assembly (загалом)** | **56** | **90** | 252 |
| `Lexer.Tokenize` | 8 | **54** ⚠️ | 38 |
| `Parser` (клас) | 18 | 72 | 79 |
| `Parser.ParsePrimary` | 4 | **61** ⚠️ | 19 |
| `Parser.ParseExpr` | 3 | 68 | 11 |
| `Parser.ParseTerm` | 3 | 68 | 11 |
| `Evaluator.Evaluate` | **9** | 81 | 23 |
| `FileExpressionSource` | 3 | 90 | 16 |
| `AstNode` | 7 | 92 | 26 |

### Пояснення метрик

- **Cyclomatic Complexity (CC)** — кількість лінійно незалежних шляхів виконання. Норма: ≤10 на метод. `Evaluator.Evaluate` = 9 (граничне значення), `Parser` = 18 (сумарно по методах — допустимо).
- **Maintainability Index (MI)** — індекс підтримуваності (0–100). Норма: ≥65. `Lexer.Tokenize` = **54** — нижче норми.
- **Class Coupling** — кількість залежностей від інших типів. Норма: ≤9. Усі класи в нормі.
- **Depth of Inheritance** — глибина ієрархії. Усі класи = 1 (тільки `object`).

---

## 3. Рефакторинг для покращення Maintainability

### Проблема 1: `Lexer.Tokenize` — MI=54 (нижче норми)

**Причина:** Великий монолітний метод з вкладеними циклами, умовами і switch-виразом.

**До:**
```csharp
public IReadOnlyList<Token> Tokenize(string expression)
{
    var tokens = new List<Token>();
    int i = 0;
    while (i < expression.Length)
    {
        char c = expression[i];
        if (char.IsWhiteSpace(c)) { i++; continue; }
        if (char.IsDigit(c) || c == '.')
        {
            int start = i;
            while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                i++;
            double val = double.Parse(expression[start..i], CultureInfo.InvariantCulture);
            tokens.Add(new Token(TokenType.Number, val));
            continue;
        }
        var type = c switch { '+' => TokenType.Plus, ... };
        tokens.Add(new Token(type));
        i++;
    }
    tokens.Add(new Token(TokenType.End));
    return tokens;
}
```

**Після** — виділено три приватні методи:
```csharp
public IReadOnlyList<Token> Tokenize(string expression)
{
    var tokens = new List<Token>();
    int i = 0;
    while (i < expression.Length)
    {
        char c = expression[i];
        if (char.IsWhiteSpace(c)) { i++; continue; }
        if (IsNumberStart(c)) { tokens.Add(ReadNumber(expression, ref i)); continue; }
        tokens.Add(new Token(MapCharToTokenType(c)));
        i++;
    }
    tokens.Add(new Token(TokenType.End));
    return tokens;
}
private static bool IsNumberStart(char c) => char.IsDigit(c) || c == '.';
private static Token ReadNumber(string expression, ref int i) { ... }
private static TokenType MapCharToTokenType(char c) => c switch { ... };
```

### Проблема 2: `Evaluator.Evaluate` — S1244 (floating point equality)

**До:** `if (divisor == 0)` — точне порівняння float  
**Після:** `if (Math.Abs(divisor) < 1e-10)` — epsilon-порівняння

---

## 4. Метрики після рефакторингу

| Клас / Метод | MI до | MI після | CC до | CC після |
|---|---|---|---|---|
| Assembly (загалом) | 90 | **91** | 56 | 58* |
| `Lexer` (клас) | 54 | **70** ✅ | 8 | 10* |
| `Evaluator.Evaluate` | 81 | **81** | 9 | 9 |

\* CC зріс через додавання нових методів (кожен метод +1 до базового CC), що є очікуваним результатом декомпозиції.

**Результат:** `Lexer` піднявся з 54 до 70 (норма ≥65 досягнута). Попередження S1244 усунуто. Загальний MI assembly покращився з 90 до 91.

---

## 5. Висновок

| Показник | До | Після |
|---|---|---|
| Sonar warnings | 1 (S1244) | **0** |
| Lexer MI | 54 ⚠️ | **70** ✅ |
| Assembly MI | 90 | **91** |
| Тести | 5/5 | **5/5** |

Рефакторинг покращив підтримуваність класу `Lexer` (Single Responsibility принцип — кожен приватний метод робить одну річ), а також усунув потенційну помилку округлення при порівнянні `double` значень.
