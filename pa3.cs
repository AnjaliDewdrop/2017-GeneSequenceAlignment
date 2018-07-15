/*
 * Name: Anjali Thatte
 * Date: November 14th
 * Purpose: PA3 - determining minimal cost gene sequence matching
 * 				- displaying the edited sequences
 */



using System;
using System.Collections.Generic;
using static System.Console;
using System.IO;

namespace Bme121
{
	class Gene
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Sequence{ get; set; }
    }
    

    class StringMatch
    {
        public enum Direction { Right, Down, Diagonal }
        
        public string S1 { get; private set; }
        public string S2 { get; private set; }
        public int[ , ] EditDistances { get; private set; }
        public List< Direction > Path { get; private set; }
        public string S1Edit { get; private set; }
        public string S2Edit { get; private set; }
        public string Bars { get; private set; }
        
        public int EditDistance
        {
            get
            {
                if( EditDistances == null ) return 0;
                else return EditDistances[ S1.Length, S2.Length ];
            }
        }
            
        public StringMatch( string s1, string s2 )
        {
            S1 = s1;
            S2 = s2;
            EditDistances = null;
            Path = null;
            S1Edit = null;
            S2Edit = null;
            Bars = null;
        }
        
        public void CalculateEditDistances( ) 
        {
            int rows = S1.Length + 1;
            int cols = S2.Length + 1;
            
            EditDistances = new int[ rows, cols ];
            
            for( int r = 0; r < rows; r ++ ) EditDistances[ r, 0 ] = r;
            for( int c = 0; c < cols; c ++ ) EditDistances[ 0, c ] = c;
            
            for( int r = 1; r < rows; r ++ )
            {
                for( int c = 1; c < cols; c ++ )
                {
                    int match = S1[ r - 1 ] == S2[ c - 1 ] ? 0 : 1;
                    int diagonal = EditDistances[ r - 1, c - 1 ] + match;
                    int down     = EditDistances[ r - 1, c     ] + 1;
                    int right    = EditDistances[ r    , c - 1 ] + 1;
                    EditDistances[ r, c ] = Math.Min( diagonal, Math.Min( down, right ) );
                }
            }
        }
            
        public void DisplayEditDistances( )
        {
            int rows = EditDistances.GetLength( 0 );
            int cols = EditDistances.GetLength( 1 );
            
            for( int c = 0; c < cols; c ++ )
            {
                if( c > 0 ) Write( "{0,3}", S2[ c - 1 ] );
                else Write( "    " );
            }
            WriteLine( );
            
            for( int r = 0; r < rows; r ++ )
            {
                if( r > 0 ) Write( "{0}", S1[ r - 1 ] );
                else Write( " " );
                
                for( int c = 0; c < cols; c ++ )
                {
                    Write( "{0,3}", EditDistances[ r, c ] );
                }
                WriteLine( );
            }
        }
        
        public void CalculatePath( )
        {
            int r = S1.Length;
            int c = S2.Length;
            
            Path = new List< Direction >( );
            
            while( r > 0 || c > 0 )
            {
                int here = EditDistances[ r, c ];
                
                int match;
                if( r > 0 && c == 0 ) match = 1;
                else if( r == 0 && c > 0 ) match = 1;
                else if( S1[ r - 1 ] != S2[ c - 1 ] ) match = 1;
                else match = 0;
                
                if( r > 0 && c > 0 && here == EditDistances[ r - 1, c - 1 ] + match )
                {
                    Path.Add( Direction.Diagonal );
                    r = r - 1;
                    c = c - 1;
                }
                else if( r > 0 && here == EditDistances[ r - 1, c ] + 1 )
                {
                    Path.Add( Direction.Down );
                    r = r - 1;
                }
                else if( c > 0 && here == EditDistances[ r, c - 1 ] + 1 )
                {
                    Path.Add( Direction.Right );
                    c = c - 1;
                }
                else
                {
                    throw new Exception( "Invalid distance matrix" );
                }
            }
            
            Path.Reverse( );
        }
        
        public void DisplayPath( )
        {
            int rows = EditDistances.GetLength( 0 );
            int cols = EditDistances.GetLength( 1 );
            
            for( int c = 0; c < cols; c ++ )
            {
                if( c > 0 ) Write( "{0,3}", S2[ c - 1 ] );
                else Write( "    " );
            }
            WriteLine( );
            
            int rShow = 0;
            int cShow = 0;
            int i = 0;
            
            for( int r = 0; r < rows; r ++ )
            {
                if( r > 0 ) Write( "{0}", S1[ r - 1 ] );
                else Write( " " );
                
                for( int c = 0; c < cols; c ++ )
                {
                    if( r == rShow && c == cShow )
                    {
                        Write( "{0,3}", EditDistances[ r, c ] );
                        if( i < Path.Count )
                        {
                                 if( Path[ i ] == Direction.Diagonal ) { rShow ++; cShow ++; }
                            else if( Path[ i ] == Direction.Down     ) { rShow ++;           }
                            else if( Path[ i ] == Direction.Right    ) {           cShow ++; }
                            i ++;
                        }
                    }
                    else Write( "  ." );
                }
                WriteLine( );
            }
        }
        
        public void CalculateEditedStrings( )
        {
			S2Edit=S2;
			S1Edit=S1;
			
			//keep track of number of gaps per sequences
			int gapCountS1=0;
			int gapCountS2=0;
			
			//initialise Bars string before modifying based on gaps
			for (int i=0; i<S1.Length; i++)
			{
				Bars+="|";
			}
			for (int i=0; i<Path.Count; i++)
			{
				if (Path[i]==Direction.Down) Bars+="|";
				if (Path[i]==Direction.Right) Bars+="|";
			}
			
			//modify sequences and bars based on positions
            for (int i=0; i<Path.Count; i++)
            {		
				
				
				if (Path[i]==Direction.Down)
				{
					S2Edit=S2Edit.Substring(0,i)+"-"+S2Edit.Substring(i);
					Bars=Bars.Substring(0,i)+" "+Bars.Substring(i+1);
					gapCountS2++;
						
						
				}
					
				else if (Path[i]==Direction.Right)
				{
					S1Edit=S1Edit.Substring(0,i)+"-"+S1Edit.Substring(i);
					Bars=Bars.Substring(0,i)+" "+Bars.Substring(i+1);
					gapCountS1++;
						
				}
				
				else if (Path[i]==Direction.Diagonal)
				{
					
					if (S1Edit[i].Equals(S2Edit[i]))
					{
						//WriteLine("match: "+S1Edit[i]+S2Edit[i]+i);     //helped me keep track of what was happening
																		  //don't want to remove commented code 
																		  //so I understand when I review in future
					}
					else
					{
						Bars=Bars.Substring(0,i)+" "+Bars.Substring(i+1);
					}
				}
					
				
			}
			Bars=Bars.Substring(0,S1Edit.Length); //remove excess bars chars
			
        }
        
        public void DisplayEditedStrings( int lineLength )
        {
            for (int i=0; i<(S1Edit.Length); i+=lineLength)
            {
				//write last line of sequences and bars (not as long as line length)
				if (S1Edit.Length-i<lineLength)
				{
					WriteLine(S1Edit.Substring(i));
					WriteLine(Bars.Substring(i));
					WriteLine(S2Edit.Substring(i));
				}
				else
				{
					WriteLine(S1Edit.Substring(i, lineLength));
					WriteLine(Bars.Substring(i, lineLength));
					WriteLine(S2Edit.Substring(i, lineLength));
				}
			}
			
        }
    }
    
    static class Program
    {
		static Gene GetGene( string fastaFile, int hmdbpNumber )
        {
            if( fastaFile == null ) return null;
            if( File.Exists( fastaFile ) == false ) return null;
            
            
            string idNum = hmdbpNumber.ToString();
            string name="";
            string seq = "";
            
            Gene result = new Gene();  //create gene object to return  
            
            using (FileStream stream = new FileStream(fastaFile, FileMode.Open, FileAccess.Read))
            using(StreamReader reader = new StreamReader(fastaFile))   //open up file
                {
                    while(!reader.EndOfStream)   //condition: not reached end of file (or found a match)
                    {
                        string line = reader.ReadLine();
                        if (line.IndexOf(idNum)>=0)  //if a matching ID is found, save the ID, name and sequence
                        {
                        
                            name = line.Substring(12);
                            while (!reader.EndOfStream)  //copy in multi-line sequence till next gene id code begins/program ends
                            {
                                line = reader.ReadLine();
                                if (line.StartsWith(">") || reader.EndOfStream)
                                {
                                    
                                    result.Number = hmdbpNumber;
                                    result.Name=name;
                                    result.Sequence=seq;
                                    return result;
                                    
                                }
                                else //go to next line
                                {
                                    seq+=line;
                                }
                            }
                        }
                    }
                }
            
            return null;    //if no match found
            
         }   
        static void Main( )
        {
            Gene gene1 = GetGene( "gene.fasta", 2077 ); // Ferritin light chain
            Gene gene2 = GetGene( "gene.fasta", 8672 ); // Ferritin heavy chain
            
            StringMatch match = new StringMatch ( gene1.Sequence, gene2.Sequence);
            match.CalculateEditDistances( );
            
            
            match.CalculatePath( );
            
            
            match.CalculateEditedStrings( );
            WriteLine( );
            
            //show edited sequences and minimal edit distance
            match.DisplayEditedStrings( 100 );
            
            WriteLine( );
            WriteLine( "Edit distance = {0}", match.EditDistance );
            WriteLine( );
            
        }
    }
}
