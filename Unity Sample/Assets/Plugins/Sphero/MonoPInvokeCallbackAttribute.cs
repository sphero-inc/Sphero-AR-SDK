using System;

/*!
 * @brief	attribute that allows static functions to have callbacks (from C) generated AOT
 */
public class MonoPInvokeCallbackAttribute : System.Attribute
{
    private Type type;
    public MonoPInvokeCallbackAttribute( Type t ) { type = t; }
}