using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables {
	//-2: final
	//-1: inicial
	//0: nada
	//1: normal
	//2: camino
	//3: suma
	//4: sustraccion
	//5: multiplicacion
	//6: division
	//7: CW
	//8: CCW

	//10X: camino con un numero especifico (X)

	//filas|columnas|posxini|posyini|imgtut	  //0    1    2    3    4    5    6  
	public static string Scene1 = "6|7|5|1|1$   1|   1|   1|   1|   1|   1|   1|" +
											"   1|   2|   2|   2|   2|  -1|   1|" +
											"   1|   2|   1|   1|   1|   1|   1|" +
											"   1|   2|   1|   0|   0|   0|   0|" +
											"   1|  -2|   1|   0|   0|   0|   0|" +
											"   1|   1|   1|   0|   0|   0|   0";
	//dadoUp/dadoLeft/dadoForward				  //0    1    2    3    4    5    6
	public static string Scene1Numbers = "1|1|1$   -4|  -7|   5|   1|   4|   4|   2|" +
												"  -3|   8|   5|   3|   2|   0|   3|" +
												"   2|   9|   9|   7|   4|   5|   4|" +
												"   3|  17|   3|   0|   0|   0|   0|" +
												"  30|   0|  32|   0|   0|   0|   0|" +
												"  14|   6|  17|   0|   0|   0|   0";
									 
	public static string Scene1Path = "1,4|1,3|1,2|1,1|2,1|3,1";

	//filas|columnas|posxini|posyini|imgtut	  //0    1    2    3    4    5    6    7    8    9 
	public static string Scene2 = "6|10|8|1|2$  0|   0|   0|   1|   1|   1|   1|   1|   1|   1|" +
											"   0|   0|   0|   1|   1|   1|   4|   1|  -1|   1|" +
											"   0|   0|   0|   1|   1|   1|   1|   1|   1|   1|" +
											"   1|   1|   1|   1|   1|   1|   0|   0|   0|   0|" +
											"   1|  -2|   1|   1|   1|   1|   0|   0|   0|   0|" +
											"   1|   1|   1|   1|   1|   1|   0|   0|   0|   0|";
	
	//filas|columnas|posxini|posyini|imgtut	  	  //0    1    2    3    4    5    6    7    8    9   
	public static string Scene2Numbers = "1|3|2$    0|   0|   0|  -4|  -7|   5|   1|   4|   4|   2|" +
												"   0|   0|   0|  -3|   6|  -1|   5|   4|   0|   3|" +
												"   0|   0|   0|   2|  -4|   6|   4|   4|   5|   4|" +
												"   3|  17|   3|  -4|  10|   5|   0|   0|   0|   0|" +
												"  30|   0| -27|  13| -14|   5|   0|   0|   0|   0|" +
												"  14|   6|  14|   2|   9|   6|   0|   0|   0|   0|";

	//filas|columnas|posxini|posyini|imgtut	  //0    1    2    3    4    5    6    7    8   
	public static string Scene3 = "7|9|7|1|0$   0|   0|   0|   0|   0|   1|   1|   1|   1|" +
											"   0|   0|   1|   1|   1|   1|   1|  -1|   1|" +
											"   0|   0|   1|   1|   4|   1|   1|   1|   1|" +
											"   0|   0|   1|   1|   1|   1|   1|   0|   0|" +
											"   1|   1|   1|   1|   1|   0|   0|   0|   0|" +
											"  -2|   1|   1|   3|   1|   0|   0|   0|   0|" +
											"   1|   1|   1|   1|   0|   0|   0|   0|   0|";

	//filas|columnas|posxini|posyini|imgtut	  	  //0    1    2    3    4    5    6    7    8  
	public static string Scene3Numbers = "1|1|1$    0|   0|   0|   0|   0|   1|   4|   4|   2|" +
												"   0|   0|  -3|   6|  -1|   3|   2|   0|   3|" +
												"   0|   0|   2|  -2|   6|   4|   4|   5|   4|" +
												"   0|   0|   3|   5|  -4|  -1|   3|   0|   0|" +
												"  -4|   3|  -4|  -7|   5|   0|   0|   0|   0|" +
												"   0|  30|  18|  12|   3|   0|   0|   0|   0|" +
												"   6|  14|   9|  15|   0|   0|   0|   0|   0|";

	//filas|columnas|posxini|posyini|imgtut	  //0    1    2    3    4    5    6    7    8 
	public static string Scene4 = "8|9|5|1|0$   0|   0|   0|   0|   1|   1|   1|   0|   0|" +
											"   0|   0|   0|   0|   1|  -1|   1|   0|   0|" +
											"   0|   0|   0|   0|   1|   1|   1|   1|   1|" +
											"   1|   1|   1|   0|   1|   7|   4|   1|   1|" +
											"   1|   1|   1|   0|   1|   1|   1|   1|   1|" +
											"  -2|   1|   4|   0|   0|   1|   1|   8|   1|" +
											"   1|   1|   8|   1|   1|   3|   1|   0|   0|" +
											"   1|   1|   1|   0|   0|   0|   0|   0|   0|";

	//filas|columnas|posxini|posyini|imgtut	      //0    1    2    3    4    5    6    7    8  
	public static string Scene4Numbers = "1|1|1$    0|   0|   0|   0|   5|   4|   2|   0|   0|" +
												"   0|   0|   0|   0|   2|   0|   1|   0|   0|" +
												"   0|   0|   0|   0|   4|   2|   5|   2|  -2|" +
												"  33|  34|  45|   0|   4|   3|   5|  -2|   4|" +
												"  23|  20|  45|   0|   5|   3|   0|   3|   3|" +
												"   0|-127| 114|   0|   0| -13|   8|  -5|  12|" +
												"  23|  45|  70|  44|  26|  18| -10|   0|   0|" +
												"  34|  28|  36|   0|   0|   0|   0|   0|   0|";

	//filas|columnas|posxini|posyini|imgtut	  //0    1    2    3    4    5    6   
	public static string Scene5 = "7|7|5|1|0$   1|   1|   1|   1|   1|   1|   1|" +
											"   1|   7|   4|   1|   1|  -1|   1|" +
											"   1|   1|   8|   1|   1|   1|   1|" +
											"   1|   1|   3|   1|   1|   8|   1|" +
											"   1|   1|   1|   1|   1|   3|   1|" +
											"   1|   7|   4|   7|   1|   1|   1|" +
											"   1|   1|   1|   1|   1|  -2|   1|";

	//filas|columnas|posxini|posyini|imgtut	 	  //0    1    2    3    4    5    6
	public static string Scene5Numbers = "1|1|1$   23|  10|   1|   5|   5|   4|   4|" +
												"  23|  -9|  11|   8|   2|   0|   4|" +
												"  23|  20|   7|   4|   3|   4|   4|" +
												" 165|  73| -16|  29|  -3|  -7|   6|" +
												"  27|   2|  -9| -10|   1|   4|  22|" +
												" 209| -14| -12|  -2|  -6|  -9|  25|" +
												"  29|  29| 210|  31|  25|   0|  25|";
}
