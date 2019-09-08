using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal.Enums;

namespace VtNetCore.VirtualTerminal
{
	/// <summary> Adapter for .NET Core <see cref="Console"/> to enable e.g. colored Output </summary>
	/// <remarks>
	/// Console exposes only few Terminal Features.
	/// Windows 10 sports a Terminal Mode which has to be activated using Win API though.
	/// There is a new MS Project: Windows Terminal which breaks the cmd.exe Behavior
	/// https://github.com/microsoft/terminal 
	/// </remarks>
	/// <example>
	/// <code>
	/// var dataLoop = new DataConsumer(new ConsoleTerminal()); 
	/// dataLoop.WriteLine("\x1b[36mTEST\x1b[0m");
	/// 
	/// </code>
	/// </example>
	public class ConsoleTerminal : IVirtualTerminalController {

		readonly List<int> _Tabs = new List<int>();
		public ConsoleColor BackgroundColor = ConsoleColor.Black;

		public ConsoleColor ForegroundColor = ConsoleColor.White;
		public IReadOnlyList<int> Tabs => _Tabs;

		public int CursorX { get; private set; }
		public int CursorY { get; private set; }

		public int Width { get; private set; }
		public int Height { get; private set; }

		public int Top { get; private set; }
		public int Left { get; private set; }

		public char LastChar { get; private set; }
		public void SetWindowTitle(string title) => Console.Title = title;

		public void SetX10SendMouseXYOnButton(bool enabled) { }
		public void SetX11SendMouseXYOnButton(bool enabled) { }

		public void ClearChanges() { } //Console.Clear(); }

		public bool IsUtf8() => Encoding.UTF8.Equals(Console.OutputEncoding);
		public void SetUTF8() => Console.OutputEncoding = Encoding.UTF8;
		public void SetLatin1() => Console.OutputEncoding = Encoding.GetEncoding("Latin-1");

		public bool IsVt52Mode() => false;

		public void Tab() => PutChar('\t');

		public void VerticalTab() => PutChar('\f');

		public void ClearTab() => _Tabs.Remove(CursorX);
		public void ClearTabs() => _Tabs.Clear();
		public void TabSet()  => _Tabs.Add(CursorX);

		public void SaveCursor() {
			CursorX = Console.CursorLeft;
			CursorY = Console.CursorTop;
		}

		public void RestoreCursor() {
			Console.CursorLeft = CursorX;
			Console.CursorTop  = CursorY;
		}

		public void ShowCursor(bool show) => Console.CursorVisible = show;

		public void MoveCursorRelative(int x, int y) {
			Console.CursorLeft += x;
			Console.CursorTop += y;
		}

		public void SetCursorPosition(int column, int row) {
			Console.CursorLeft = column;
			Console.CursorTop  = row;
		}

		public void SetCursorStyle(ECursorShape shape, bool blink) {
			Console.CursorSize = AsCursorSize(shape);
			//Need Windows.Cursor DLL to control blinking 
		}

		public void XTermDeiconifyWindow() => Console.SetWindowSize(Width, Height);

		public void XTermIconifyWindow() => Console.SetWindowSize(0, 0);

		public void XTermFullScreenToggle() => XTermMaximizeWindow(true, true);

		public void XTermMaximizeWindow(bool horizontally, bool vertically) {
			int top; 
			int left; 
			int width;
			int height;
			if (horizontally) {
				left = 0;
				if (0 != Console.WindowLeft) {
					Left = Console.WindowLeft;
				}
				width = Console.LargestWindowWidth;
				if (Console.LargestWindowWidth != Console.WindowWidth) {
					Width = Console.WindowWidth;
				}
			} else {
				left = Left;
				width = Width;
			}
			if (vertically) {
				top = 0;
				if (0 != Console.WindowLeft) {
					Top = Console.WindowTop;
				}
				height = Console.LargestWindowHeight;
				if (Console.LargestWindowHeight != Console.WindowHeight) {
					Height = Console.WindowHeight;
				}
			} else {
				top = Top;
				height = Height;
			}
			Console.SetWindowPosition(left, top);
			Console.SetWindowSize(width, height);
		}


		public void XTermMoveWindow(int x, int y) => Console.SetWindowPosition(x, y);
		public void XTermResizeWindow(int width, int height) => Console.SetWindowSize(width, height);

		public void XTermFullScreenEnter() {
			XTermMaximizeWindow(true, true);
			Top = Console.WindowTop ; Console.WindowTop = 0;
			Left= Console.WindowLeft; Console.WindowLeft = 0;
			Height = Console.WindowHeight; Console.WindowHeight = Console.LargestWindowHeight;
			Width= Console.WindowWidth ; Console.WindowWidth = Console.LargestWindowWidth;
		}

		public void XTermFullScreenExit() {
			XTermMaximizeWindow(false, false);
			Console.WindowTop	= Top ;
			Console.WindowLeft	= Left;
			Console.WindowHeight = Width;
			Console.WindowWidth = Height;
		}

		public void InsertBlanks(int count) => PutChar(' ', count);
		public void InsertColumn(int count) => PutChar('\t', count);
		public void InsertLines(int count) => PutChar('\n', count);

		public void Bell() => PutChar('\a'); //Console.Beep();
		public void Backspace() => PutChar('\b');
		public void FormFeed() => PutChar('\f');
		public void CarriageReturn() => PutChar('\r');
		public void NewLine() => PutChar('\n');

		public void ClearScrollingRegion() { }

		public void DeleteCharacter(int count) {
		}

		public void DeleteColumn(int count) {
		}

		public void DeleteLines(int count) {
		}

		public void DeviceStatusReport() { }

		public void EnableAutoRepeatKeys(bool enable) {
			//TODO: need Windows.Keyboard for this
		}

		public void Enable80132Mode(bool enable) { }
		public void Enable132ColumnMode(bool enable) { }
		public void EnableAlternateBuffer() { }
		public void EnableApplicationCursorKeys(bool enable) { }
		public void EnableBlinkingCursor(bool enable) { }
		public void EnableLeftAndRightMarginMode(bool enable) { }
		public void EnableNationalReplacementCharacterSets(bool enable) { }
		public void EnableNormalBuffer() { }
		public void EnableOriginMode(bool enable) { }
		public void EnableReverseVideoMode(bool enable) { }
		public void EnableReverseWrapAroundMode(bool enable) { }
		public void EnableSmoothScrollMode(bool enable) { }
		public void EnableSgrMouseMode(bool enable) { }
		public void EnableUrxvtMouseMode(bool enabled) { }
		public void EnableWrapAroundMode(bool enable) { }

		public void EraseAll(bool ignoreProtected) => Console.Clear();
		public void EraseAbove(bool ignoreProtected) { }
		public void EraseBelow(bool ignoreProtected) { }

		public void EraseCharacter(int count) { }
		public void EraseLine(bool ignoreProtected) { }
		public void EraseToEndOfLine(bool ignoreProtected) { }
		public void EraseToStartOfLine(bool ignoreProtected) { }

		public void FullReset() {
			Console.ForegroundColor = ForegroundColor;
			Console.BackgroundColor = BackgroundColor;
		}

		public void InvokeCharacterSetMode(ECharacterSetMode mode) { }
		public void InvokeCharacterSetModeR(ECharacterSetMode mode) { }

		public void PopXTermWindowIcon() { }
		public void PopXTermWindowTitle() { }
		public void ProtectCharacter(int protect) { }
		public void PushXTermWindowIcon() { }
		public void PushXTermWindowTitle() { }

		public void PutChar(char character) => Console.Write(LastChar = character);
		public void PutG2Char(char character) => PutChar(character);
		public void PutG3Char(char character) => PutChar(character);

		public void RepeatLastCharacter(int count) => PutChar(LastChar, count);

		public void RequestDecPrivateMode(int mode) { }
		public void RequestStatusStringSetConformanceLevel() { }
		public void RequestStatusStringSetProtectionAttribute() { }
		public void ReportCursorPosition() {
			//TODO: write the Position to the Console. Input Stream Buffer;
		}

		public void ReportExtendedCursorPosition() { }
		public void ReportRgbBackgroundColor() { }
		public void ReportRgbForegroundColor() { }

		public void RestoreEnableNormalBuffer() { }
		public void RestoreEnableSgrMouseMode() { }
		public void RestoreUseCellMotionMouseTracking() { }
		public void RestoreUseHighlightMouseTracking() { }
		public void RestoreBracketedPasteMode() { }
		public void RestoreCursorKeys() { }
		public void ReverseIndex() { }
		public void ReverseTab() { }

		public void SetAutomaticNewLine(bool enable) { }

		public void SaveBracketedPasteMode() { }

		public void SaveCursorKeys() { }

		public void SaveEnableNormalBuffer() { }

		public void SaveEnableSgrMouseMode() { }

		public void SaveUseCellMotionMouseTracking() { }

		public void SaveUseHighlightMouseTracking() { }

		public void Scroll(int rows) { }
		public void ScrollAcross(int columns) { }

		public void SendDeviceAttributes() { }
		public void SendDeviceAttributesSecondary() { }
		public void SendDeviceAttributesTertiary() { }


		public void SetAbsoluteRow(int line) { }

		public void SetBracketedPasteMode(bool enable) { }

		public void SetCharacterAttribute(int parameter) { }
		public void SetCharacterSet(ECharacterSet characterSet, ECharacterSetMode mode) { }
		public void SetCharacterSize(ECharacterSize size) { }
		public void SetConformanceLevel(int level, bool eightBit) { }

		public void SetEndOfGuardedArea() { }
		public void SetErasureMode(bool enabled) { }
		public void SetGuardedAreaTransferMode(bool enabled) { }

		public void SetInsertReplaceMode(EInsertReplaceMode mode) { }
		public void SetReplaceMode(bool isReplaceMode) { }

		public void SetIso8613PaletteBackground(int paletteEntry) { }
		public void SetIso8613PaletteForeground(int paletteEntry) { }

		public void SetLeftAndRightMargins(int left, int right) { }

		public void SetKeypadType(EKeypadType type) { }

		public void SetRgbBackgroundColor(int red, int green, int blue) {
			//Console.BackgroundColor = ; supports only 8 dark and 8 bright colors...
		}
		public void SetRgbBackgroundColor(string xParseColor) {
			//Console.BackgroundColor = ; TODO: ...could try to find the closest Match
		}
		public void SetRgbForegroundColor(int red, int green, int blue) {
			//Console.ForegroundColor = ;
		}
		public void SetRgbForegroundColor(string xParseColor) {
			//Console.ForegroundColor = ;
		}

		public void SetScrollingRegion(int top, int bottom) { }

		public void SetSendFocusInAndFocusOutEvents(bool enabled) { }

		public void SetStartOfGuardedArea() { }

		public void SetUseAllMouseTracking(bool enabled) { }

		public void SetUtf8MouseMode(bool enabled) { }

		public void SetVt52AlternateKeypadMode(bool enabled) { }

		public void SetVt52GraphicsMode(bool enabled) { }

		public void SetVt52Mode(bool enabled) { }

		public void SetX10SendMouseXyOnButton(bool enabled) { }

		public void SetX11SendMouseXyOnButton(bool enabled) { }

		public void ShiftIn() { }

		public void ShiftOut() { }

		public void SingleShiftSelectG2() { }

		public void SingleShiftSelectG3() {
			//Console.;
		}

		public void UseCellMotionMouseTracking(bool enable) { }

		public void UseHighlightMouseTracking(bool enable) { }


		public void Vt52EnterAnsiMode() { }


		public void Vt52Identify() { }

		public void XTermLowerToBottom() {
			//Console.;
		}

		public void XTermRaiseToFront() {
			//Console.Focus;
		}

		public void XTermRefreshWindow() {
			//Console.;
		}

		public void XTermReport(XTermReportType reportType) { }

		public void XTermResizeTextArea(int columns, int rows) { }

		void PutChar(char chr, int count) {
			LastChar = chr;
			for (var i = count; --i >= 0;) {
				Console.Write(chr);
			}
		}

		static int AsCursorSize(ECursorShape shape) {
			switch (shape) {
				case ECursorShape.Block:     return 100;
				case ECursorShape.Underline: return 10;
				case ECursorShape.Bar:       return 30;
				default:                             throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
			}
		}

	}

}