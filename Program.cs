using System;
using ILOG.Concert;
using ILOG.CPLEX;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;


namespace Midterm_Project
{
    class Program 
    {
        static void Main(string[] args)
        {
            StreamReader Set_j = new StreamReader(@"read_csv\product.csv");
            List<string> J = new List<string>();
            string Set_line;
            while ((Set_line = Set_j.ReadLine()) != null)
            {
                J.Add(Set_line);
            }
            Set_j.Close();

            //0~24
            StreamReader Set_t2 = new StreamReader(@"read_csv\period_0_24.csv");
            List<string> T2 = new List<string>();
            while ((Set_line = Set_t2.ReadLine()) != null)
            {
                T2.Add(Set_line);
            }
            Set_t2.Close();

            //1~24
            StreamReader Set_t = new StreamReader(@"read_csv\period.csv");
            List<string> T1 = new List<string>();
            while ((Set_line = Set_t.ReadLine()) != null)
            {
                T1.Add(Set_line);
            }
            Set_t.Close();
            

            StreamReader Set_sale = new StreamReader(@"read_csv\salesprice.csv");
            double[] sale = new double[J.Count];
            for (int j = 0; j < J.Count; j++)
            {
                sale[j] = Convert.ToDouble(Set_sale.ReadLine());
            }
            Set_sale.Close();

            StreamReader Parameter_file = new StreamReader(@"read_csv\demand.csv");
            double[][] demand = new double[J.Count][];
            for (int i = 0; i < J.Count; i++)
            {
                demand[i] = new double[T1.Count];
            }
            string Parameter_line;
            while ((Parameter_line = Parameter_file.ReadLine()) != null)
            {
                string[] temp_line = Parameter_line.Split(',');
                demand[int.Parse(temp_line[0]) - 1][int.Parse(temp_line[1]) - 1] = Convert.ToDouble(temp_line[2]);
            }

            Parameter_file.Close();


            Cplex cplexmodel = new Cplex();

            //Decision variables

            INumVar[][][] x = new INumVar[6][][];
            for(int i = 0; i < 6; i++)
            {
                x[i] = new INumVar[J.Count][];
                if(i == 0)
                {
                    for (int j = 0; j < J.Count; j++)
                    {
                        
                        if (j > 993 && j < 2131)
                        {
                            
                            x[i][j] = cplexmodel.NumVarArray(T1.Count,0.0, System.Double.MaxValue);
                            
                        }
                        else
                        {
                            
                            x[i][j] = cplexmodel.NumVarArray(T1.Count,0.0, 0.0);
                            
                        }
                    }
                }

                if (i == 1)
                {
                    for (int j = 0; j < J.Count; j++)
                    {
                        
                        if (j > 2130 && j < 3028)
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count,0.0, System.Double.MaxValue);
                            
                        }
                        else
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count,0.0, 0.0);
                            
                        }
                    }
                }

                if (i == 2)
                {
                    for (int j = 0; j < J.Count; j++)
                    {
                        
                        if (j > 993 && j < 3395)
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, 0.0);
                            
                        }
                        else
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, System.Double.MaxValue);
                            
                        }
                    }
                }

                if (i == 3)
                {
                    for (int j = 0; j < J.Count; j++)
                    {
                        x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, System.Double.MaxValue);
                        
                    }
                }

                if (i == 4)
                {
                    for (int j = 0; j < J.Count; j++)
                    {
                        
                        if (j >= 0 && j < 2131)
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, System.Double.MaxValue);
                            
                        }
                        else
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, 0.0);
                            
                        }
                    }
                }

                if (i == 5)
                {
                    for(int j = 0; j < J.Count; j++)
                    {
                        if (j > 2130 && j < 3395)
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, System.Double.MaxValue);
                            
                        }
                        else
                        {
                            x[i][j] = cplexmodel.NumVarArray(T1.Count, 0.0, 0.0);
                            
                        }
                    }
                }
            }
         

            INumVar[][] s = new INumVar[J.Count][];
            for(int j = 0; j < J.Count; j++)
            {
                s[j] = cplexmodel.NumVarArray(T1.Count, 0.0, double.MaxValue);
            }

            INumVar[][] b = new INumVar[J.Count][];
            for(int j = 0; j < J.Count; j++)
            {
                b[j] = cplexmodel.NumVarArray(T2.Count, 0.0, double.MaxValue);
            }

            INumVar[][] I = new INumVar[J.Count][];
            for(int j = 0; j < J.Count; j++)
            {
                I[j] = cplexmodel.NumVarArray(T2.Count, 0.0, double.MaxValue);
            }

            //Objective
            ILinearNumExpr TotalRevenue = cplexmodel.LinearNumExpr();
            ILinearNumExpr TotalProdcost = cplexmodel.LinearNumExpr();
            ILinearNumExpr TotalInvcost = cplexmodel.LinearNumExpr();
            ILinearNumExpr TotalBackcost = cplexmodel.LinearNumExpr();
            for(int j = 0; j < J.Count; j++)
            {
                for(int t = 0; t < T1.Count; t++)
                {
                    for(int i = 0; i < 6; i++)
                    {
                        TotalProdcost.AddTerm(0.5 * sale[j], x[i][j][t]);
                    }
                }
            }
            for(int j = 0; j < J.Count; j++)
            {
                for(int t = 0; t < T1.Count; t++)
                {
                    TotalRevenue.AddTerm(sale[j], s[j][t]);
                }
            }
            for(int j = 0; j < J.Count; j++)
            {
                for(int t = 0; t < T1.Count; t++)
                {
                    TotalInvcost.AddTerm(1, I[j][t + 1]);
                    TotalBackcost.AddTerm(2 * sale[j], b[j][t + 1]);
                }
            }
            cplexmodel.AddMaximize(cplexmodel.Diff(TotalRevenue, cplexmodel.Sum(TotalInvcost, TotalBackcost, TotalProdcost)));


            //Constraints



            //constraint 1
            
            for (int t = 1; t < T2.Count; t++)
            {
                for(int j = 0; j < J.Count; j++)
                {
                    ILinearNumExpr Inventory = cplexmodel.LinearNumExpr();
                    for (int i = 0; i < 6; i++)
                    {
                        Inventory.AddTerm(1, x[i][j][t-1]);
                    }
                    cplexmodel.AddEq(I[j][t], cplexmodel.Diff(cplexmodel.Sum(I[j][t-1], b[j][t], Inventory), 
                        cplexmodel.Sum(b[j][t - 1], demand[j][t - 1])));
                }
            }


            //constraint 2
            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr1 = cplexmodel.LinearNumExpr();
                for (int j = 994; j < 2131; j++)
                {
                    availExpr1.AddTerm(1, x[0][j][t]);
                }
                cplexmodel.AddLe(availExpr1, 45000);
            }

            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr2 = cplexmodel.LinearNumExpr();
                for (int j = 2131; j < 3028; j++)
                {
                    availExpr2.AddTerm(1, x[1][j][t]);
                }
                cplexmodel.AddLe(availExpr2, 30000);
            }

            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr3_1 = cplexmodel.LinearNumExpr();
                ILinearNumExpr availExpr3_2 = cplexmodel.LinearNumExpr();
                for (int j = 0; j < 994 ; j++)
                {
                    availExpr3_1.AddTerm(1, x[2][j][t]);
                }
                for (int j = 3395; j < 3412; j++)
                {
                    availExpr3_2.AddTerm(1, x[2][j][t]);
                }
                cplexmodel.AddLe(cplexmodel.Sum(availExpr3_1, availExpr3_2), 40000);
            }

            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr4 = cplexmodel.LinearNumExpr();
                for (int j = 0; j < 3412; j++)
                {
                    availExpr4.AddTerm(1, x[3][j][t]);
                }
                cplexmodel.AddLe(availExpr4, 18000);
            }

            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr5 = cplexmodel.LinearNumExpr();
                for (int j = 0; j < 2131; j++)
                {
                    availExpr5.AddTerm(1, x[4][j][t]);
                }
                cplexmodel.AddLe(availExpr5, 20000);
            }

            for (int t = 0; t < T1.Count; t++)
            {
                ILinearNumExpr availExpr6 = cplexmodel.LinearNumExpr();
                for (int j = 2131; j < 3395; j++)
                {
                    availExpr6.AddTerm(1, x[5][j][t]);
                }
                cplexmodel.AddLe(availExpr6, 17000);
            }

            //constraint 3
            
            
            for (int j = 0; j < J.Count; j++)
            {
                for (int t = 0; t < T1.Count; t++)
                {
                    ILinearNumExpr accuSales = cplexmodel.LinearNumExpr();
                    double d = 0;
                    for(int t0 = 0; t0 < t + 1; t0++)
                    {
                        accuSales.AddTerm(1, s[j][t0]);
                        d = demand[j][t0] + d;
                    }
                    cplexmodel.AddLe(accuSales, d);
                }
            }
         
            //constraint 4
            
            for(int t = 1; t < T2.Count; t++)
            {
                for(int j = 0; j < J.Count; j++)
                {
                    ILinearNumExpr product = cplexmodel.LinearNumExpr();
                    for (int i = 0; i < 6; i++)
                    {
                        product.AddTerm(1, x[i][j][t-1]);
                    }
                    cplexmodel.AddLe(s[j][t - 1], cplexmodel.Sum(I[j][t-1], product));
                }
            }


            //constraint 5
            for(int j = 0; j < J.Count; j++)
            {
                cplexmodel.AddEq(I[j][0], 0);
            }

            //constraint 6
            for(int j = 0; j < J.Count; j++)
            {
                cplexmodel.AddEq(b[j][0], 0);
            }

            cplexmodel.ExportModel("Result.lp");
            cplexmodel.Solve();


            //Output 
            
            Console.WriteLine("Max profit = " + cplexmodel.GetObjValue());
            Console.WriteLine();
            
            
            double[] Xijt = new double[T1.Count];
            double[] Sjt = new double[T1.Count];
            double[] Bjt = new double[T1.Count];
            double[] ijt = new double[T1.Count];
            

            StreamWriter sales = new StreamWriter(@"result_sales.csv");
            StreamWriter prod = new StreamWriter(@"result_prod.csv");
            StreamWriter bg = new StreamWriter(@"result_backlog.csv");
            StreamWriter inven = new StreamWriter(@"result_inventory.csv");


            sales.Write(",");
            inven.Write(",");
            bg.Write(",");

            for (int t = 1; t <= T1.Count; t++)
            {
                sales.Write("T=" + t + ",");
                bg.Write("T=" + t + ",");
                inven.Write("T=" + t + ",");
            }

            sales.WriteLine();
            bg.WriteLine();
            inven.WriteLine();
            

            prod.Write("i ,j ,t, Xijt");
            prod.WriteLine();
            for (int j = 0; j < J.Count; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    Xijt = cplexmodel.GetValues(x[i][j]);
                    for (int y = 0; y < Xijt.Length; y++)
                    {
                        if (Xijt[y] != 0)
                        {
                            prod.Write("{0},{1},{2},{3}", i + 1, j + 1, y + 1, Xijt[y]);
                            prod.WriteLine();

                        }
                    }
                }
            }


            for(int j = 0; j < J.Count; j++)
            {
                Sjt = cplexmodel.GetValues(s[j]);
                sales.Write("J = " + (j + 1) + ",");
                for(int y = 0; y < T1.Count; y++)
                {
                    sales.Write("{0},", Sjt[y]);
                }
                sales.WriteLine();
            }

            for (int j = 0; j < J.Count; j++)
            {
                ijt = cplexmodel.GetValues(I[j]);
                inven.Write("J = " + (j + 1) + ",");
                for (int y = 1; y < T2.Count; y++)
                {
                    inven.Write("{0},", ijt[y]);
                }
                inven.WriteLine();
            }

            for (int j = 0; j < J.Count; j++)
            {
                Bjt = cplexmodel.GetValues(b[j]);
                bg.Write("J = " + (j + 1) + ",");
                for (int y = 1; y < T2.Count; y++)
                {
                    bg.Write("{0},", Bjt[y]);
                }
                bg.WriteLine();
            }

            prod.Close();
            inven.Close();
            bg.Close();
            sales.Close();
            
            
            cplexmodel.End();
        }

    }
}


