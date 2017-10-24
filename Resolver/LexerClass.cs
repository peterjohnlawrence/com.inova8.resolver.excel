using System;
namespace Resolver
{
	
	public class LexerClass
	{
		virtual protected internal int LineNumber
		{
			get
			{
				int getLineNumber = 0;
				try
				{
					getLineNumber = pLineNumber;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getLineNumber");
				}
				return getLineNumber;
			}
			
		}
		virtual protected internal int Offset
		{
			get
			{
				int getOffset = 0;
				try
				{
					getOffset = pOffSet;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getOffset");
				}
				return getOffset;
			}
			
		}
		virtual protected internal int LineOffset
		{
			get
			{
				int getLineOffset = 0;
				try
				{
					getLineOffset = pLineOffset;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getLineOffset");
				}
				return getLineOffset;
			}
			
		}
		virtual protected internal System.String Lexeme
		{
			get
			{
				System.String getLexeme = "";
				try
				{
					getLexeme = pLexeme;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getLexeme");
				}
				return getLexeme;
			}
			
		}
		virtual protected internal System.String Consumer
		{
			set
			{
				try
				{
					int i = 0;
					int lenProblem = 0;
					//lenProblem = Strings.Len( txtProblem );
					lenProblem = value.Length;
					pProblem = new char[lenProblem + 1 + 1];
					for (i = 1; i <= lenProblem; i++)
					{
						// pProblem[(int)(i)] = (byte)(Strings.Asc( Strings.Mid( txtProblem, (int)(i), 1 ) ));
						pProblem[i] = value[i - 1];
					}
					pProblem[i] = (char) ((sbyte) 0);
					pLineNumber = 1;
					pLineOffset = 1;
					pOffSet = 0;
					GetChar();
					//Else
					//    Err.Description = "Parse error. Problem size (" & lenProblem & " ) too large. Maximum allowed is " & bufferlength
					//    Err.Raise vbObjectError + 100
					//End If
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "setConsumer");
				}
			}
			
		}
		/// <summary>64000 </summary>
		//private int bufferlength = 512000;
		private int pLineNumber = 0;
		private int pOffSet = 0;
		private int pLineOffset = 0;
		/// <summary>Private pProblem(1 To bufferlength) As Byte </summary>
		private char[] pProblem = null;
		private char bChar = (char) (0);
		private System.String pLexeme = "";
		private TokenClass pPrevious = new TokenClass();
		private int AscEOF = 0;
		private int AscTAB = 9;
		private int AscLF = 10;
		private int AscCR = 13;
		private int AscSPACE = 32;
		// public Variant Empty = new Variant();
		private System.String Empty = "";
		
		protected internal virtual void  GetChar()
		{
			try
			{
				bChar = pProblem[pOffSet + 1];
				pLexeme = System.Convert.ToString((char) (bChar));
				pOffSet = pOffSet + 1;
				pLineOffset = pLineOffset + 1;
			}
			catch (System.Exception _e_)
			{
				Err.set_Renamed(_e_, "GetChar");
			}
		}
		
		protected internal virtual bool Analyze(TokenClass NextToken)
		{
			bool Analyze = false;
			try
			{
				
				NextToken.Value = Empty.ToString();
				NextToken.Found = false;
				Analyze = true;
				
				do 
				{
					
					//if( (bChar >= (byte)(Strings.Asc( "a" )) && bChar <= (byte)(Strings.Asc( "z" ))) || (bChar >= (byte)(Strings.Asc( "A" )) && bChar <= (byte)(Strings.Asc( "Z" ))) )
					if ((bChar >= 'a' && bChar <= 'z') || (bChar >= 'A' && bChar <= 'Z'))
					{
						//capital alpha, start of keyword
						do {
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
						}
						//while( !(! (((bChar >= Strings.Asc( "a" )) && (bChar <= Strings.Asc( "z" ))) || ((bChar >= Strings.Asc( "A" )) && (bChar <= Strings.Asc( "Z" ))) || ((bChar >= Strings.Asc( "0" )) && (bChar <= Strings.Asc( "9" ))) || (bChar == Strings.Asc( "_" )) || (bChar == Strings.Asc( "@" ))) || (bChar == AscEOF)) );
						while (!(!(((bChar >= 'a') && (bChar <= 'z')) || ((bChar >= 'A') && (bChar <= 'Z')) || ((bChar >= '0') && (bChar <= '9')) || (bChar == '_') || (bChar == '@')) || (bChar == AscEOF)));
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexId;
					}
					else if ((bChar >= (sbyte) ('0') && bChar <= (sbyte) ('9')))
					{
						//digit start of a number
						do 
						{
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
						}
						while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
						
						//., therefore start of a real
						if (bChar == '.')
						{
							do 
							{
								NextToken.Value = NextToken.Value + pLexeme;
								GetChar();
							}
							while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
							// e, therefore an exponential
							if ((bChar == 'e') || (bChar == 'E'))
							{
								NextToken.Value = NextToken.Value + pLexeme;
								GetChar();
								if (((bChar >= '0') && (bChar <= '9')) || (bChar == '+') || (bChar == '-'))
								{
									do 
									{
										NextToken.Value = NextToken.Value + pLexeme;
										GetChar();
									}
									while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
									NextToken.Found = true;
									NextToken.LexType = Initialize.ALexReal;
								}
								else
								{
									Analyze = false;
									Err.Description = "Lexical error. Found illegal numeric '" + System.Convert.ToString((char) (bChar)) + "' at line " + pLineNumber + ", offset " + pLineOffset;
									Err.raise(100, "", "");
                                    Err.raise(100, "Analyze", Err.Description, System.Convert.ToString((char)(bChar)), pLineNumber, pLineOffset);
                                }
							}
							else
							{
								NextToken.Found = true;
								//NextToken.LexType = ALexInteger
								NextToken.LexType = Initialize.ALexReal;
							}
						}
						else if ((bChar == 'e') || (bChar == 'E'))
						{
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
							if (((bChar >= '0') && (bChar <= '9')) || (bChar == '+') || (bChar == '-'))
							{
								do 
								{
									NextToken.Value = NextToken.Value + pLexeme;
									GetChar();
								}
								while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
								NextToken.Found = true;
								NextToken.LexType = Initialize.ALexReal;
							}
						}
						else
						{
							NextToken.Found = true;
							//NextToken.LexType = ALexInteger
							NextToken.LexType = Initialize.ALexReal;
						}
					}
					else if (bChar == (sbyte) ('$'))
					{
						do 
						{
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
						}
						while (!(!(((bChar >= 'a') && (bChar <= 'z')) || ((bChar >= 'A') && (bChar <= 'Z'))) || (bChar == AscEOF)));
						
						if (NextToken.Value.Equals("$NORMAL"))
						{
							NextToken.LexType = Initialize.ALexNORMAL;
							NextToken.Found = true;
						}
						else
						{
							
						}
					}
					else if (bChar == (sbyte) ('('))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexOParen;
					}
					else if (bChar == (sbyte) (')'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexCParen;
					}
					else if (bChar == (sbyte) (','))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexComma;
					}
					else if (bChar == (sbyte) (':'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexColon;
					}
					else if (bChar == (sbyte) (';'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexSemiColon;
					}
					else if (bChar == (sbyte) ('='))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexEqual;
					}
					else if (bChar == (sbyte) ('+'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexPlus;
					}
					else if (bChar == (sbyte) ('-'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexMinus;
					}
					else if (bChar == (sbyte) ('*'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexMult;
					}
					else if (bChar == (sbyte) ('^'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexPower;
					}
					else if (bChar == (sbyte) ('.'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						if ((bChar >= '0') && (bChar <= '9'))
						{
							do 
							{
								NextToken.Value = NextToken.Value + pLexeme;
								GetChar();
							}
							while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
							// e, therefore an exponential
							if ((bChar == 'e') || (bChar == 'E'))
							{
								NextToken.Value = NextToken.Value + pLexeme;
								GetChar();
								if (((bChar >= '0') && (bChar <= '9')) || (bChar == '+') || (bChar == '-'))
								{
									do 
									{
										NextToken.Value = NextToken.Value + pLexeme;
										GetChar();
									}
									while (!(!(((bChar >= '0') && (bChar <= '9'))) || (bChar == AscEOF)));
									NextToken.Found = true;
									NextToken.LexType = Initialize.ALexReal;
								}
								else
								{
									Analyze = false;
									Err.Description = "Lexical error. Found illegal numeric '" + System.Convert.ToString((char) (bChar)) + "' at line " + pLineNumber + ", offset " + pLineOffset;
                                    Err.raise(100, "Analyze", Err.Description, System.Convert.ToString((char)(bChar)), pLineNumber, pLineOffset);
								}
							}
							else
							{
								NextToken.Found = true;
								NextToken.LexType = Initialize.ALexReal;
							}
						}
						else
						{
							//Case Asc(".")
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexDot;
						}
					}
					else if (bChar == (sbyte) ('<'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						if (bChar == '=')
						{
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexLE;
						}
						else
						{
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexLT;
						}
					}
					else if (bChar == (sbyte) ('>'))
					{
						NextToken.Value = pLexeme;
						GetChar();
						if (bChar == '=')
						{
							NextToken.Value = NextToken.Value + pLexeme;
							GetChar();
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexGE;
						}
						else
						{
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexGT;
						}
					}
					else if (bChar == (sbyte) (AscSPACE) || bChar == (sbyte) (AscTAB) || bChar == (sbyte) (AscCR))
					{
						GetChar();
					}
					else if (bChar == (sbyte) (AscLF))
					{
						//linefeed
						pLineOffset = 1;
						pLineNumber = pLineNumber + 1;
						GetChar();
					}
					else if (bChar == (sbyte) ('/'))
					{
						///, start of comment
						NextToken.Value = pLexeme;
						GetChar();
						//single line comment
						if (bChar == '/')
						{
							do 
							{
								GetChar();
								//end of single line comment
							}
							while (!((bChar == AscLF) || (bChar == AscEOF)));
							NextToken.Value = Empty.ToString();
							//start of multiline comment
						}
						else if (bChar == '*')
						{
							bool EndOfComment = false;
							EndOfComment = false;
							do 
							{
								do 
								{
									GetChar();
								}
								while (!((bChar == 10) || (bChar == 42) || (bChar == AscEOF)));
								if (bChar == 10)
								{
									pLineNumber = pLineNumber + 1;
									pLineOffset = 1;
								}
								else if (bChar == '*')
								{
									GetChar();
									//end of multiline comment
									if (bChar == '/')
									{
										EndOfComment = true;
										GetChar();
									}
								}
							}
							while (!((EndOfComment == true) || (bChar == AscEOF)));
							NextToken.Value = Empty.ToString();
						}
						else
						{
							//Case Asc("/")
							NextToken.Found = true;
							NextToken.LexType = Initialize.ALexDiv;
						}
					}
					else if (bChar == (sbyte) (AscEOF))
					{
						//EOF
						NextToken.Found = true;
						NextToken.LexType = Initialize.ALexEOF;
					}
					else
					{
						Err.Description = "Lexical error. Found illegal character '" + pLexeme + "' at line " + pLineNumber + ", offset " + pLineOffset;
                        Err.raise(100, "Analyze", Err.Description, pLexeme, pLineNumber, pLineOffset);
					}
				}
				while (!(NextToken.Found));
			}
			catch (System.Exception _e_)
			{
				Err.set_Renamed(_e_, "Analyze");
			}
			return Analyze;
		}
		
		protected internal virtual void  Match(int Token, TokenClass LookAhead)
		{
			try
			{
				//Token As LexType
				if (LookAhead.LexType == Token)
				{
					pPrevious = LookAhead;
					Analyze(LookAhead);
				}
				else
				{
					Err.Description = "Parse error. Found '" + LookAhead.Value + "', expecting '" + Initialize.ALexString(Token) + "' at line " + pLineNumber + ", offset " + pLineOffset;
                    Err.raise(100, "Analyze", Err.Description, LookAhead.Value, pLineNumber, pLineOffset);
                    Err.raise(100, "", "");
				}
			}
			catch (System.Exception _e_)
			{
				Err.set_Renamed(_e_, "Match");
			}
		}
		
		public LexerClass()
		{
			try
			{
				pPrevious.LexType = Initialize.ALexEOF;
			}
			catch (System.Exception _e_)
			{
				Err.set_Renamed(_e_, "LexerClass");
			}
		}
	}
}