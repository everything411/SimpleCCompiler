using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.CodeGeneration
{
    public static class CodeGenerator
    {
        public static string Template= @"
.386
.model flat, stdcall
option casemap :none
include windows.inc
include user32.inc
include kernel32.inc
include gdi32.inc
include msvcrt.inc
includelib gdi32.lib
includelib user32.lib
includelib kernel32.lib
includelib msvcrt.lib
.const 
	GetIntFmt db ""%d"",0
	PrintStrFmt db ""%s"",10,0
	PrintIntFmt db ""%d"",10,0
{0}
.data
	dummy dd 0
.data?
	dummy2 dd ?
.CODE
Mars_GetInt:
	push ebp
	mov ebp, esp
	sub esp, 4
	lea eax, [ebp - 4]
	push eax
	push offset GetIntFmt
	call crt_scanf
	add esp, 8
	mov eax, [ebp - 4]
	mov esp, ebp
	pop ebp
	ret
Mars_PrintInt:
	push ebp
	mov ebp, esp
	push DWORD ptr[ebp + 8]
	push offset PrintIntFmt
	call crt_printf
	add esp, 8
	mov esp, ebp
	pop ebp
	ret
Mars_PrintStr:
	push ebp
	mov ebp, esp
	push DWORD ptr[ebp + 8]
	push offset PrintStrFmt
	call crt_printf
	add esp, 8
	mov esp, ebp
	pop ebp
	ret
{1}
START:
	call main
	push eax
	call ExitProcess
end START
";

		public static string GenerateAssembly(Module module)
        {
			List<IRStringLiteral> stringLiterals = new();
            foreach (Function item in module.Functions)
            {
				stringLiterals.AddRange(item.StringLiterals);
			}
			StringBuilder stringLiteralsBuilder = new();
            foreach (var item in stringLiterals)
            {
				stringLiteralsBuilder.AppendLine($"{item.LiteralName} db {item.Value},0");
			}
			StringBuilder assemblyBuilder = new();
            foreach (var item in module.Instructions)
            {
				assemblyBuilder.AppendLine(item.EmitAssembly());
            }
			return string.Format(Template, stringLiteralsBuilder.ToString(), assemblyBuilder.ToString());
        }
		public static void GenerateAssemblyFile(string filename, Module module)
        {
			File.WriteAllText(filename, GenerateAssembly(module));
        }
	}
}
