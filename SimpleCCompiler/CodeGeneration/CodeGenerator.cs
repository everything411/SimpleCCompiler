using SimpleCCompiler.IR;
using System;
using System.Collections.Generic;
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
.data
	dummy dd 0
.data?
	dummy2 dd?
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
{0}
START:
	call main
	push eax
	call ExitProcess
end START
";
		public static string GenerateAssembly(List<IIR> IRs)
        {
			StringBuilder sourceCodeBuilder = new();
            foreach (var item in IRs)
            {
				sourceCodeBuilder.AppendLine(item.ToString());
            }
			return string.Format(Template, sourceCodeBuilder.ToString());
        }
	}
}
