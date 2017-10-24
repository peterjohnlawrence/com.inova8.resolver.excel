 [BNF] for the formula expression is :
 <expression> ::= <term> [<addop> <term>]*
 <term> ::= <factor>  [ <mulop> <factor> ]*
 <factor> ::= <number> | (<expression>) | <cellRef> | <function>

 <function> ::= <functionName> ([expression [, expression]*])
Formula
 Formula ::= "=" <Expression>

 ArrayFormula? ::= "=" "{" <Expression> "}"
        An array formula is a formula that performs multiple calculations on one or more sets of
        values, and then returns either a single result or multiple results. Array formulas are 
        enclosed between braces { }, and are entered by pressing CTRL+SHIFT+ENTER.
 Expression ::= <Primitive> | <Cell> | <Function> | ...
        Formulas can refer to cells or ranges of cells, or to names or labels that represent cells or ranges

 ReferencePrefix? ::= (<Workbook> | <Sheet> | e ) <Reference>

	e represents the empty string/null
 Reference ::= <Cell> | <AreaReference?>
 AreaReference? ::= <Range> | <Vector> | <Intersection> | <Union>

 Workbook ::= "["workbook_path/filename.xls"]" <Sheet>
 Sheet ::= <Sheetname>"!"
 Sheetname ::= <The name of each worksheet.> e.g. Sheet1, Sheet2, Sheet3, ...

 3DSheetReference? ::= <Sheetname><UnionOperator?><Sheet><Cell>
        E.g. =COUNTA(Sheet1:Sheet3!A1)

 Cell ::= ["$"]<Column>["$"]<Row>
	The $ symbol denotes an absolute reference to that dimension/axis.
 Row ::= x | x e Int,  0 < x = 65536
	Row numbers are in [1..65536]

 Column ::= [A-Z] | [A-H][A-Z] | I[A-Z]
	Here [A-Z] is interupted like a regular expression i.e. Any character between 'A' and 'Z'. Columns use letters (A through IV, for a total of 256 columns) for labels.
Absolute/Relative? references

Absolute Column, Absolute Row, $A$1, is an absolute cell reference
Relative Column, Absolute Row, A$1, is a mixed cell reference
Absolute Column, Relative Row, $A1, is a mixed cell reference
Relative Column, Relative Row, A1, is a relative cell reference (default style)

Pressing the F4 key in Excel, while typing the cell address, will cycle it through the four variations.

R1C1 references

[Ref]. To toggle between A1 and R1C1 style referencing Tools > Options > General tab > R1C1 reference style.

In R1C1 style, Microsoft Excel indicates the location of a cell with an "R" followed by a row number and a "C" followed by a column number. For example, the absolute cell reference R1C1 is equivalent to the absolute reference $A$1 in A1 reference style. If the active cell is A1, the relative cell reference R[1]C[1] refers to the cell one row down and one column to the right, or B2.

The following are examples of references in R1C1 style.

Reference Meaning

 R[-2]C	  A relative reference to the cell two rows up and in the same column
 R[2]C[2] A relative reference to the cell two rows down and two columns to the right
 R2C2	  An absolute reference to the cell in the second row and in the second column
 R[-1]	  A relative reference to the entire row above the active cell
 R	  An absolute reference to the current row
AreaReference? operators
 Range ::= <Cell><RangeOperator?><Cell>

 Vector ::= <ColumnRange?> | <RowRange?>
	It is possible to refer to a single column(B:B) or row(5:5) range (known as a vector) in Excel.
 ColumnRange? ::= <Column><RangeOperator?><Column>
	The first column reference should be = to the second column reference.
 RowRange? ::= <Row><RangeOperator?><Row>
	The first row reference should be = to the second row reference.
 Intersection ::= <AreaReference?><IntersectionOperator?><AreaReference?>
	Two ranges seperated by a single space.
 Union ::= <AreaReference?><UnionOperator?><AreaReference?>
	Reference operators combine ranges of cells for calculations.
Operators
(Based on [WSFF] and help files from [EXCEL])

Operators specify the type of calculation that you want to perform on the elements of a formula. Microsoft Excel includes four different types of calculation operators: arithmetic, comparison, text, and reference.

 Operator ::=  <ComparrisonOperator?> | <ArithmaticOperator?> | <ConcatinationOperator?>

 ComparrisonOperator? ::= ">=" | "<=" | "=" | "=" | "<>" | "="
	Comparison operators compare two values and then produce the logical value TRUE or FALSE.
        (greater than sign) Greater than. E.g. A1>B1
        (less than sign) Less than. E.g. A1<B
        (greater than or equal to sign) Greater than or equal to. E.g. A1>=B1 
        (less than or equal to sign) Less than or equal to. E.g. A1<=B1
        (not equal to sign) Not equal to. E.g. A1<>B1
        (equal sign) Equal to. E.g. A1=B1

 ArithmaticOperator? ::= <ExpressionOperator?> | <TermOperator?> | <ExponentiationOperator?> | <PercentageOperator?>

        Arithmetic operators perform basic mathematical operations such as addition, subtraction, or multiplication; combine numbers; and produce numeric results.
 ExpressionOperator? ::= "+" | "-"
        Arithmetic operators perform basic mathematical operations such as addition, subtraction, or multiplication; combine numbers; and produce numeric results.
        + (plus sign) Addition.      E.g. 3+3
        – (minus sign) Subtraction.  E.g. 3–1
 TermOperator? ::= "*" | "/"
        * (asterisk) Multiplication. E.g. 3*3
        / (forward slash) Division.  E.g. 3/3

 NegationOperator? ::= "-"
        – (minus sign) Negation.     E.g. –1
 ExponentiationOperator? ::= "^"
        ^ (caret) Exponentiation.    E.g. 3^2
 PercentageOperator? ::= "%"
        % (percent sign) Percent.    E.g. 20%
 ConcatinationOperator? ::= "&"
	(ampersand) Connects, or concatenates, two values to produce one continuous text value. E.g. "North" & "wind" produces "Northwind"

 ReferenceOperator? ::= <RangeOperator?> | <UnionOperator?> | <IntersectionOperator?>

	Reference operators combine ranges of cells for calculations.
 RangeOperator? ::= ":"
	(colon) produces one reference to all the cells between two references, including the two references. E.g. B5:B15   
 UnionOperator? ::= ","
        (comma) combines multiple references into one reference. E.g. SUM(B5:B15,D5:D15) 

 IntersectionOperator? ::= " "
        (single space) produces one reference to cells common to two references. E.g. SUM(B5:B15 A7:D7) In this example, cell B7 is common to both ranges. 
Operator Precedence

Based on [Excel Tutorial - SC 2000 - Basic Math] ([local]) and [Microsoft support].

When you combine several operators into a single formula, Microsoft Excel performs the operations in the following order:
: Range RangeOperator?
space Intersection IntersectionOperator?
, Union UnionOperator?
- Negation NegationOperator?
% Percentage PercentageOperator?
^ Exponentiation ExponentiationOperator?
* Multiplication or / Division ExpressionOperator?
+ Addition or - Subtraction TermOperator?
& Text Operator ConcatinationOperator?
= < > <= >= <> Comparison Operators ComparrisonOperator?
In Excel 97 Help: operators, evaluation order in formulas "The order in which Microsoft Excel performs operations in formulas" topic: operators, precedence...

 Note: (From http://www.faqs.org/faqs/spreadsheets/faq/)

 10.13 Why is =-1^2 positive and =0-1^2 negative?

  nonstandard operator precedence -- the `unary minus' has a high
  precedence, as normal, but the `exponentiation operator' has a
  higher one, which is NOT normal.
  Excel treats -1^2 as (-1)^2, while most languages would as -(1^2).
  (you can ALWAYS use parentheses to force either interpretation.)
  Note that Excel formulas and VBA formulas disagree on the order.
Primitives
 Primitive ::= Number | Boolean | String | Date
Functions
To get list of functions in catagory. Find in help for "about function". Cut and paste list of functions to Word and do replace using ^wworksheet function^p with |
Then do replace using ^t with nothing

 Function ::= (Function_LOGICAL | Function_FINANCIAL | Function_STATISTICAL | Function_MATHANDTRIG | 
 Function_DATABASE | Function_DATEANDTIME | Function_ENGINEERING | Function_TEXT | 
 Function_INFORMATION | Function_LOOKUPANDREFERENCE)

 Function_LOOKUPANDREFERENCE ::= ("ADDRESS" | "AREAS" | "CHOOSE" | "COLUMN" | "COLUMNS" | "HLOOKUP" | "HYPERLINK" | "INDEX" | "INDIRECT" | "LOOKUP" | "MATCH" | "OFFSET" | "ROW" | "ROWS" | "TRANSPOSE" | "VLOOKUP")
 Function_INFORMATION ::= ("CELL" | "COUNTBLANK" | "ERROR.TYPE" | "INFO" | "ISBLANK" | "ISERR" | "ISERROR" | "ISEVEN" | "ISLOGICAL" | "ISNA" | "ISNONTEXT" | "ISNUMBER" | "ISODD" | "ISREF" | "ISTEXT" | "N" | "NA" | "TYPE")

 Function_FINANCIAL ::= ("ACCRINT" | "ACCRINTM" | "AMORDEGRC" | "AMORLINC" | "COUPDAYBS" | "COUPDAYS" | "COUPDAYSNC" | "COUPNCD" | "COUPNUM" | "COUPPCD" | "CUMIPMT" | "CUMPRINC" | "DB" | "DDB" | "DISC" | "DOLLARDE" | "DOLLARFR" | "DURATION" | "EFFECT" | "FV" | "FVSCHEDULE" | "INTRATE" | "IPMT" | "IRR" | "MDURATION" | "MIRR" | "NOMINAL" | "NPER" | "NPV" | "ODDFPRICE" | "ODDFYIELD" | "ODDLPRICE" | "ODDLYIELD" | "PMT" | "PPMT" | "PRICE" | "PRICEDISC" | "PRICEMAT" | "PV" | "RATE" | "RECEIVED" | "SLN" | "SYD" | "TBILLEQ" | "TBILLPRICE" | "TBILLYIELD" | "VDB" | "XIRR" | "XNPV" | "YIELD" | "YIELDDISC" | "YIELDMAT")
 Function_ENGINEERING ::= ("BESSELI" | "BESSELJ" | "BESSELK" | "BESSELY" | "BIN2DEC" | "BIN2HEX" | "BIN2OCT" | "COMPLEX" | "CONVERT" | "DEC2BIN" | "DEC2HEX" | "DEC2OCT" | "DELTA" | "ERF" | "ERFC" | "GESTEP" | "HEX2BIN" | "HEX2DEC" | "HEX2OCT" | "IMABS" | "IMAGINARY" | "IMARGUMENT" | "IMCONJUGATE" | "IMCOS" | "IMDIV" | "IMEXP" | "IMLN" | "IMLOG10" | "IMLOG2" | "IMPOWER" | "IMPRODUCT" | "IMREAL" | "IMSIN" | "IMSQRT" | "IMSUB" | "IMSUM" | "OCT2BIN" | "OCT2DEC" | "OCT2HEX")
 Function_DATEANDTIME ::= ("DATE" | "DATEVALUE" | "DAY" | "DAYS360" | "EDATE" | "EOMONTH" | "HOUR" | "MINUTE" | "MONTH" | "NETWORKDAYS" | "NOW" | "SECOND" | "TIME" | "TIMEVALUE" | "TODAY" | "WEEKDAY" | "WORKDAY" | "YEAR" | "YEARFRAC")

 Function_DATABASE ::= ("DAVERAGE" | "DCOUNT" | "DCOUNTA" | "DGET" | "DMAX" | "DMIN" | "DPRODUCT" | "DSTDEV" | "DSTDEVP" | "DSUM" | "DVAR" | "DVARP" | "GETPIVOTDATA")
 Function_MATHANDTRIG ::= ("ABS" | "ACOS" | "ACOSH" | "ASIN" | "ASINH" | "ATAN" | "ATAN2" | "ATANH" | "CEILING" | "COMBIN" | "COS" | "COSH" | "COUNTIF" | "DEGREES" | "EVEN" | "EXP" | "FACT" | "FACTDOUBLE" | "FLOOR" | "GCD" | "INT" | "LCM" | "LN" | "LOG" | "LOG10" | "MDETERM" | "MINVERSE" | "MMULT" | "MOD" | "MROUND" | "MULTINOMIAL" | "ODD" | "PI" | "POWER" | "PRODUCT" | "QUOTIENT" | "RADIANS" | "RAND" | "RANDBETWEEN" | "ROMAN" | "ROUND" | "ROUNDDOWN" | "ROUNDUP" | "SERIESSUM" | "SIGN" | "SIN" | "SINH" | "SQRT" | "SQRTPI" | "SUBTOTAL" | "SUMIF"+")" | "("+"SUM" | "SUMPRODUCT" | "SUMSQ" | "SUMX2MY2" | "SUMX2PY2" | "SUMXMY2" | "TAN" | "TANH" | "TRUN")

 Function_STATISTICAL ::= ("AVEDEV" | "AVERAGE" | "AVERAGEA" | "BETADIST" | "BETAINV" | "BINOMDIST" | "CHIDIST" | "CHIINV" | "CHITEST" | "CONFIDENCE" | "CORREL" | "COUNT" | "COUNTA" | "COVAR" | "CRITBINOM" | "DEVSQ" | "EXPONDIST" | "FDIST" | "FINV" | "FISHER" | "FISHERINV" | "FORECAST" | "FREQUENCY" | "FTEST" | "GAMMADIST" | "GAMMAINV" | "GAMMALN" | "GEOMEAN" | "GROWTH" | "HARMEAN" | "HYPGEOMDIST" | "INTERCEPT" | "KURT" | "LARGE" | "LINEST" | "LOGEST" | "LOGINV" | "LOGNORMDIST" | "MAX" | "MAXA" | "MEDIAN" | "MIN" | "MINA" | "MODE" | "NEGBINOMDIST" | "NORMDIST" | "NORMINV" | "NORMSDIST" | "NORMSINV" | "PEARSON" | "PERCENTILE" | "PERCENTRANK" | "PERMUT" | "POISSON" | "PROB" | "QUARTILE" | "RANK" | "RSQ" | "SKEW" | "SLOPE" | "SMALL" | "STANDARDIZE" | "STDEV" | "STDEVA" | "STDEVP" | "STDEVPA" | "STEYX" | "TDIST" | "TINV" | "TREND" | "TRIMMEAN" | "TTEST" | "VAR" | "VARA" | "VARP" | "VARPA" | "WEIBULL" | "ZTEST")
 Function_TEXT ::= ("CHAR" | "CLEAN" | "CODE" | "CONCATENATE" | "DOLLAR" | "EXACT" | "FIND" | "FIXED" | "LEFT" | "LEN" | "LOWER" | "MID" | "PROPER" | "REPLACE" | "REPT" | "RIGHT" | "SEARCH" | "SUBSTITUTE" | "T" | "TEXT" | "TRIM" | "UPPER" | "VALUE")
 Function_LOGICAL ::= ("IF" | "AND" | "OR" | "NOT" | "TRUE" | "FALSE");