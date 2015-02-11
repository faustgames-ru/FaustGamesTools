grammar CppClasses;

/*
 * Parser Rules
 */

namespaceDeclaration
    :   'namespace' namespaceName 
        namespaceBody
    ;

namespaceName
	: Identifier
	;

namespaceBody
    :   '{' structDeclaration* classDeclaration* methodDeclaration* '}'
	;

structDeclaration
    :   'struct' structDeclarationName 
        structBody
    ;

structDeclarationName
	: Identifier
	;

structBody
    :   '{' fieldsDeclarations* '}' ';'
    ;

classDeclaration
    :   'class' classDeclarationName 
        (':' classExtends)?
        classBody
    ;

classDeclarationName
	: Identifier
	;

classExtends
	:	classNames
	;

classNames
	: className (',' className)?
	;

className
	: Identifier
	;

classBody
    :   '{' 'public'':' methodDeclaration* '}' ';'
    ;

methodDeclaration
	:	('extern' '"C"' 'DLLEXPORT')? 'virtual'? typeParameterConst? returnType typeParameterLink? call? methodName '(' methodParameters? ')' methodPure? ';'
	;

methodPure
	: '=''0'
	;

methodName
	: Identifier
	;

returnType 
	: type
	;

call
	:	Identifier 
	;

type
    :   Identifier ('.' Identifier)*
    ;

methodParameters
    :   methodParameter (',' methodParameter)*
    ;
	
methodParameter
    :   typeParameterConst? parameterType typeParameterLink? parameterName?
    ;

typeParameterConst
	:	'const'
	;

typeParameterLink
	:	'*'
	;

parameterType
	:	type
	;

parameterName
	:	variableDeclaratorId
	;

fieldsDeclarations
    :   fieldDeclaration (',' fieldDeclaration)*
    ;
	
fieldDeclaration
    :   fieldType fieldName ';'
    ;

fieldType
	:	type
	;

fieldName
	:	variableDeclaratorId
	;

variableDeclaratorId
    :   Identifier ('[' ']')*
    ;

Identifier
    :   IdentifierNondigit
        (   IdentifierNondigit
        |   Digit
        )*
    ;

fragment
IdentifierNondigit
    :   Nondigit
    |   UniversalCharacterName
    //|   // other implementation-defined characters...
    ;

fragment
Nondigit
    :   [a-zA-Z_]
    ;

fragment
Digit
    :   [0-9]
    ;

fragment
UniversalCharacterName
    :   '\\u' HexQuad
    |   '\\U' HexQuad HexQuad
    ;

fragment
HexQuad
    :   HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit
    ;

fragment
HexadecimalDigit
    :   [0-9a-fA-F]
    ;

compileUnit
	:	EOF
	;

/*
 * Lexer Rules
 */

 WS  :  [ \t\r\n\u000C]+ -> skip
    ;

COMMENT
    :   '/*' .*? '*/' -> skip
    ;

LINE_COMMENT
    :   '//' ~[\r\n]* -> skip
    ;

DIRECTIVE
    :   '#' ~[\r\n]* -> skip
    ;

TYPEDEF
    :   'typedef' ~[\r\n]* -> skip
    ;