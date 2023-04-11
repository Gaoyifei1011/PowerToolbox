#include "DPICalcHelper.h"

/// <summary>
/// ��Ч����ֵת��Ϊʵ�ʵ�����ֵ
/// </summary>
int DPICalcHelper::ConvertEpxToPixel(HWND hwnd, int effectivePixels)
{
	float scalingFactor = GetScalingFactor(hwnd);
	return (int)(effectivePixels * scalingFactor);
}

/// <summary>
/// ʵ�ʵ�����ֵת��Ϊ��Ч����ֵ
/// </summary>
int DPICalcHelper::ConvertPixelToEpx(HWND hwnd, int pixels)
{
	float scalingFactor = GetScalingFactor(hwnd);
	return (int)(pixels / scalingFactor);
}

/// <summary>
/// ��ȡʵ�ʵ�ϵͳ���ű���
/// </summary>
float DPICalcHelper::GetScalingFactor(HWND hwnd)
{
	int dpi = GetDpiForWindow(hwnd);
	float scalingFactor = (float)dpi / 96;
	return scalingFactor;
}