using System.Globalization;
using System.Text.RegularExpressions;
using Compiler.CodeProcessing.AbstractSyntaxTree.Nodes;
using Compiler.CodeProcessing.CompilationStructuring;

namespace Compiler.CodeProcessing.IntermediateAssembyLanguage;

public static class OpCode
{
    public static IntermediateInstruction Nop() => new(Instruction.Nop, []);

    public static IntermediateInstruction LdConst_int(string type, long value) => new(Instruction.LdConst, [type, value.ToString()]);
    public static IntermediateInstruction LdConst_string(string value) => new(Instruction.LdConst, ["str", value]);
    public static IntermediateInstruction LdConst_float(string type, double value) => new(Instruction.LdConst, [type, value.ToString(CultureInfo.InvariantCulture)]);
    public static IntermediateInstruction LdConst_bool(bool value) => new(Instruction.LdConst, ["bool", value.ToString()]);

    public static IntermediateInstruction SetLocal(int id) => new(Instruction.SetLocal, [id.ToString()]);
    public static IntermediateInstruction SetLocal(LocalRef lr) => new(Instruction.SetLocal, [lr.index.ToString()]);
    public static IntermediateInstruction GetLocal(int id) => new(Instruction.GetLocal, [id.ToString()]);
    public static IntermediateInstruction GetLocal(LocalRef lr) => new(Instruction.GetLocal, [lr.index.ToString()]);

    public static IntermediateInstruction Add() => new(Instruction.Add, []);
    public static IntermediateInstruction Sub() => new(Instruction.Sub, []);
    public static IntermediateInstruction Mul() => new(Instruction.Mul, []);
    public static IntermediateInstruction Div() => new(Instruction.Div, []);
    public static IntermediateInstruction Rest() => new(Instruction.Rest, []);

    public static IntermediateInstruction Conv(string type) => new(Instruction.Conv, [type]);

    public static IntermediateInstruction CallStatic(MethodItem method) => new(Instruction.CallStatic, [method.returnType.ToAsmString(), method.GetGlobalReferenceIL()]);

    public static IntermediateInstruction Ret() => new(Instruction.Ret, []);

    public static IntermediateInstruction Jump(int offset) => new(Instruction.Jump, [offset.ToString()]);
    
    public static IntermediateInstruction If(ConditionMethod condition)
        => new(Instruction.If, [condition.ToString()]);
    public static IntermediateInstruction If(ConditionMethod condition, string value)
        => new(Instruction.If, [condition.ToString(), value]);
    public static IntermediateInstruction Else()
        => new(Instruction.Else, []);
    public static IntermediateInstruction EndIf()
        => new(Instruction.EndIf, []);
}

public readonly struct IntermediateInstruction(Instruction instruction, string[] parameters)
{
    public readonly Instruction instruction = instruction;
    public readonly string[] parameters = parameters;

    public override string ToString()
    {
        string strInst = instruction switch
        {
            Instruction.Nop             => "nop",

            Instruction.SetField        => "setField.{0}",
            Instruction.SetLocal        => "setLocal.{0}",
            Instruction.GetField        => "getField.{0}",
            Instruction.GetLocal        => "getLocal.{0}",

            Instruction.LdConst         => parameters[0] != "str" ? "ldConst.{0}" : "ldConst.{0} \"{1}\"",

            Instruction.Add             => "add",
            Instruction.Sub             => "sub",
            Instruction.Mul             => "mul",
            Instruction.Div             => "div",
            Instruction.Rest            => "rest",

            Instruction.Conv            => "conv.{0}",

            Instruction.CallStatic      => "call.Static {0} {1}",
            Instruction.CallInstance    => "call.Instance {0} {1}",
            Instruction.CallVirtual     => "call.Virtual {0} {1}",

            Instruction.Jump             => "jump",
            Instruction.If               => "if.{0}",
            Instruction.Else             => "else",
            Instruction.EndIf            => "endif",

            Instruction.Ret             => "ret",

            _ => throw new NotSupportedException(instruction.ToString())
        };

        var vc = GetVariableCount(strInst);

        strInst = string.Format(strInst, parameters[0..vc]);
        return $"{strInst} {string.Join(", ", parameters[vc..])}";
    }

    private static int GetVariableCount(string formatString)
    {
        return Regex.Matches(formatString, @"\{(\d+)\}")
            .Cast<Match>()
            .Select(m => int.Parse(m.Groups[1].Value))
            .DefaultIfEmpty(-1)
            .Max() + 1;
    }
}

public enum Instruction : byte
{
    Nop,

    SetField,
    SetLocal,

    GetField,
    GetLocal,

    LdConst,

    Add,
    Sub,
    Mul,
    Div,
    Rest,

    Conv,

    CallStatic,
    CallInstance,
    CallVirtual,

    Jump,
    If,
    Else,
    EndIf,
    Ret
}

public enum ConditionMethod : byte
{
    Forced,

    Greater,
    Lesser,
    GreaterEqual,
    LesserEqual,

    Equal,

    True,
    False,

    Zero,

}
