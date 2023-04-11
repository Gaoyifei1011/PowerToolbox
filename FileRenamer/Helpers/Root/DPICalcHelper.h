#pragma once

#include <Windows.h>

/// <summary>
/// DPI��ÿӢ����������ż��㸨����
/// </summary>
class DPICalcHelper
{
public:
	static int ConvertEpxToPixel(HWND hwnd, int effectivePixels);
	static int ConvertPixelToEpx(HWND hwnd, int pixels);
private:
	static float GetScalingFactor(HWND hwnd);
};
