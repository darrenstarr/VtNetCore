namespace VtNetCore.VirtualTerminal
{
    using VtNetCore.VirtualTerminal.Enums;

    /// <summary>
    /// Provides the interfaces needed by a terminal in order to manipulate the model.
    /// </summary>
    public interface IVirtualTerminalController
    {
        /// <summary>
        /// Clears the change tracking of the model.
        /// </summary>
        /// <remarks>
        /// This is not really well placed here.
        /// </remarks>
        void ClearChanges();

        void Backspace();
        void Bell();
        void CarriageReturn();
        void ClearScrollingRegion();
        void ClearTab();
        void ClearTabs();
        void DeleteCharacter(int count);
        void DeleteColumn(int count);
        void DeleteLines(int count);
        void DeviceStatusReport();
        void Enable80132Mode(bool enable);
        void Enable132ColumnMode(bool enable);
        void EnableAlternateBuffer();
        void EnableApplicationCursorKeys(bool enable);
        void EnableAutoRepeatKeys(bool enable);
        void EnableBlinkingCursor(bool enable);
        void EnableLeftAndRightMarginMode(bool enable);
        void EnableNormalBuffer();
        void EnableOriginMode(bool enable);
        void EnableReverseVideoMode(bool enable);
        void EnableReverseWrapAroundMode(bool enable);
        void EnableSmoothScrollMode(bool enable);
        void EnableSgrMouseMode(bool enable);
        void EnableWrapAroundMode(bool enable);
        void EraseAbove();
        void EraseAll();
        void EraseBelow();
        void EraseCharacter(int count);
        void EraseLine();
        void EraseToEndOfLine();
        void EraseToStartOfLine();
        void FormFeed();
        void FullReset();
        void InsertBlanks(int count);
        void InsertColumn(int count);
        void InsertLines(int count);
        void MoveCursorRelative(int x, int y);
        void NewLine();
        void PutChar(char character);
        void ReportCursorPosition();
        void RestoreCursor();
        void RestoreEnableNormalBuffer();
        void RestoreEnableSgrMouseMode();
        void RestoreUseCellMotionMouseTracking();
        void RestoreUseHighlightMouseTracking();
        void RestoreBracketedPasteMode();
        void RestoreCursorKeys();
        void ReverseIndex();
        void ReverseTab();
        void SetAutomaticNewLine(bool enable);
        void SaveBracketedPasteMode();
        void SaveCursor();
        void SaveCursorKeys();
        void SaveEnableNormalBuffer();
        void SaveEnableSgrMouseMode();
        void SaveUseCellMotionMouseTracking();
        void SaveUseHighlightMouseTracking();
        void Scroll(int rows);
        void SendDeviceAttributes();
        void SendDeviceAttributesSecondary();
        void SetAbsoluteRow(int line);
        void SetBracketedPasteMode(bool enable);
        void SetCharacterAttribute(int parameter);
        void SetCharacterSet(ECharacterSet characterSet);
        void SetCharacterSize(ECharacterSize size);
        void SetCursorPosition(int column, int row);
        void SetInsertReplaceMode(EInsertReplaceMode mode);
        void SetLatin1();
        void SetLeftAndRightMargins(int left, int right);
        void SetKeypadType(EKeypadType type);
        void SetWindowTitle(string title);
        void SetScrollingRegion(int top, int bottom);
        void SetUTF8();
        void ShiftIn();
        void ShiftOut();
        void ShowCursor(bool show);
        void Tab();
        void TabSet();
        void UseCellMotionMouseTracking(bool enable);
        void UseHighlightMouseTracking(bool enable);
        void VerticalTab();
    }
}
