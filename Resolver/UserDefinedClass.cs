using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.inova8.resolver
{
    /*  The equations are organized as 'nodes' in a binary directed graph
     *  So for a user defined function with multiple arguments such as 'USERDEF(arg1,arg1,arg3)'
     *  the nodal structure would be as follows:
     *  
     *                                                  |                         
     *                       Node(operatorType:Function,functionType:USERDEF,operand:NULL)
     *                      /                                                              \                   
     *                     Left                                                             Right
     *                    /                                                                  \ 
     *    Node(operatorType:ptrVariable,functionType:USERDEF,operand:arg1)                  Node(operatorType:ListSeparator,functionType:USERDEF,operand:NULL)
     *   /                                                               \                  /                                                              \                   
     *  -                                                                 -                Left                                                             Right
     *                                                                                    /                                                                  \ 
     *                                                                       Node(operatorType:ptrVariable,functionType:USERDEF,operand:arg2)               Node(operatorType:ListSeparator,functionType:USERDEF,operand:NULL)
     *                                                                      /                                                                \              /                                                              \
     *                  -                                                  -                                                                  -            Left                                                             -
     *                                                                                                                                                    /    
     *                                                                                                                                     Node(operatorType:ptrVariable,functionType:USERDEF,operand:arg3)   
     *                                                                                                                                     /                                                            \
     *                                                                                                                                    -                                                              -
     * 
     *  Resolver needs to evaulate both the user defined function and the derivative of that function at current conditions with respect to a particular variable (wrt). 
     *  To perform some statistical analysis, Resolver also needs to evaluate the user defined function and the derivative of that function with respect to a particular variable (wrt) at initial conditions. 
     *  
     */
    class UserFunctions
    {
        /*
         *  SUM(arg1, arg2, arg3, ...) where argn can be a literal, variable, another expression, or a range
         *  Evaluated recursively as follows:
         *  evaluate = arg1 + evaluate(arg2,arg3, ...)
         *  derivative = d.arg1 + derivative(arg2,arg3, ...)
         */
        internal double SUM_Evaluate(NodeClass thisNode, Constants.FunctionType functionContext)
        {
            double evaluate = thisNode.Left.Evaluate(functionContext);
            if (thisNode.Right != null)
            {
                evaluate += thisNode.Right.Evaluate(functionContext);
            }
            return evaluate;
        }
        internal double SUM_EvaluateResidual(NodeClass thisNode, Constants.FunctionType functionContext)
        {
            double evaluateResidual = thisNode.Left.EvaluateResidual(functionContext);
            if (thisNode.Right != null)
            {
                evaluateResidual += thisNode.Right.EvaluateResidual(functionContext);
            }
            return evaluateResidual;
        }
        internal double SUM_Derivative(NodeClass thisNode, CellClass wrt, Constants.FunctionType functionContext)
        {
            double derivative = thisNode.Left.Derivative(wrt, functionContext);
            if (thisNode.Right != null)
            {
                derivative += thisNode.Right.Derivative(wrt, functionContext); ;
            }
            return derivative;
        }
        internal double SUM_DerivativeResidual(NodeClass thisNode, CellClass wrt, Constants.FunctionType functionContext)
        {
            double derivativeResidual = thisNode.Left.DerivativeResidual(wrt, functionContext);
            if (thisNode.Right != null)
            {
                derivativeResidual += thisNode.Right.DerivativeResidual(wrt, functionContext);
            }
            return derivativeResidual;
        }
        /*
         *  PRODUCT(arg1, arg2, arg3, ...)  where argn can be a literal, variable, another expression, or a range
         *  Evaluated recursively as follows:
         *  evaluate = arg1 * evaluate(arg2,arg3, ...)
         *  derivative = arg1 * derivative(arg2,arg3, ...) +  d.arg1 * evaluate(arg2,arg3, ...) 
         */
        internal double PRODUCT_Evaluate(NodeClass thisNode, Constants.FunctionType functionContext)
        {
            double evaluate = thisNode.Left.Evaluate(functionContext);
            if (thisNode.Right != null)
            {
                evaluate *= thisNode.Right.Evaluate(functionContext);
            }
            return evaluate;
        }
        internal double PRODUCT_EvaluateResidual(NodeClass thisNode, Constants.FunctionType functionContext)
        {
            double evaluateResidual = thisNode.Left.EvaluateResidual(functionContext);
            if (thisNode.Right != null)
            {
                evaluateResidual *= thisNode.Right.EvaluateResidual(functionContext);
            }
            return evaluateResidual;
        }
        internal double PRODUCT_Derivative(NodeClass thisNode, CellClass wrt, Constants.FunctionType functionContext)
        {
            double leftDerivative = thisNode.Left.Derivative(wrt, functionContext);
            double derivative = 0.0;
            if (thisNode.Right != null)
            {
                derivative = (thisNode.Left.Evaluate(functionContext) * thisNode.Right.Derivative(wrt, functionContext)) + (leftDerivative * thisNode.Right.Evaluate(functionContext));
            }
            return derivative;
        }
        internal double PRODUCT_DerivativeResidual(NodeClass thisNode, CellClass wrt, Constants.FunctionType functionContext)
        {
            double leftDerivativeResidual = thisNode.Left.Derivative(wrt, functionContext);
            double derivativeResidual =0.0;
            if (thisNode.Right != null)
            {
                double rightDerivativeResidual = thisNode.Right.DerivativeResidual(wrt, functionContext);
                derivativeResidual = (thisNode.Left.EvaluateResidual(functionContext) * rightDerivativeResidual) + (leftDerivativeResidual * thisNode.Right.EvaluateResidual(functionContext));
            }
            return derivativeResidual;
        }
        /*
         *  USERDEF(arg1, arg2, arg3, ...) where argn can be a literal, variable, another expression, or a range
         *  Example shown is simply the identity I(arg1), any further arguments being ignored. 
        */
        internal  double USERDEF_Evaluate(NodeClass thisNode,Constants.FunctionType functionContext)
        {
            //Evaluate,  the function evaluated  at the current values of the reconciled variables
            return thisNode.Left.Evaluate(functionContext);
        }
        internal  double USERDEF_EvaluateResidual(NodeClass thisNode,Constants.FunctionType functionContext)
        {
            //EvaluateResidual, the function evaluated at the initial conditions of the problem which really means the initial measurements
            return thisNode.Left.EvaluateResidual(functionContext);
        }
        internal  double USERDEF_Derivative(NodeClass thisNode,CellClass wrt, Constants.FunctionType functionContext)
        {
            //Derivative(wrt) the derivative of the function with respect to one of the reconciled variables evaluated at the current values of the reconciled variables
            return thisNode.Left.Derivative(wrt, functionContext);
        }
        internal  double USERDEF_DerivativeResidual(NodeClass thisNode,CellClass wrt, Constants.FunctionType functionContext)
        {
            //DerivativeResidual(wrt) the derivative function  with respect to one of the reconciled variables evaluated at the initial conditions of the problem which really means the initial measurements
            return thisNode.Left.DerivativeResidual(wrt, functionContext);
        }
    }
}
