#pragma once

#include <string>
#include <Windows.h>
#include <WinMain.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.UI.Xaml.h>

#include "Helpers/Root/DPICalcHelper.h"
#include "MileWindow.h"
#include "MainPage.xaml.h"

using namespace std;
using namespace winrt;
using namespace winrt::Windows::Graphics;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Controls;

WNDPROC MileOldWndProc = 0;
LRESULT CALLBACK MileNewWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

MileWindow::MileWindow()
{
	MileWindow::Position = { 0,0 };
	MileWindow::Size = { 0,0 };
	MileWindow::MinWindowSize = { 0,0 };
	MileWindow::MaxWindowSize = { 0,0 };
	MileWindow::_handle = NULL;
	MileWindow::_isWindowCreated = false;
}

bool MileWindow::IsWindowCreated()
{
	return _isWindowCreated;
}
void MileWindow::IsWindowCreated(bool value)
{
	_isWindowCreated = value;
}

string MileWindow::Title()
{
	return _title;
}
void MileWindow::Title(string value)
{
	_title = value;
}

HWND MileWindow::Handle()
{
	return _handle;
}
void MileWindow::Handle(HWND value)
{
	_handle = value;
}

UIElement MileWindow::Content()
{
	return _content;
}
void MileWindow::Content(UIElement value)
{
	_content = value;
}

/// <summary>
/// ��ʼ��Ӧ�ô���
/// </summary>
void MileWindow::InitializeWindow(HINSTANCE hInstance)
{
	HWND hwnd = CreateWindowExW(
		WS_EX_LEFT,
		L"Mile.Xaml.ContentWindow",
		L"FileRenamer",
		WS_OVERLAPPEDWINDOW,
		MileWindow::Position.X,
		MileWindow::Position.Y,
		MileWindow::Size.X,
		MileWindow::Size.Y,
		nullptr,
		nullptr,
		hInstance,
		get_abi(MileWindow::Content()));

	MileWindow::Handle(hwnd);
	if (MileWindow::Handle() == nullptr)
	{
		throw "HelperResources/WindowHandleInitializeFailed";
	}
	else
	{
		MileOldWndProc = (WNDPROC)SetWindowLongPtr(MileWindow::Handle(), GWLP_WNDPROC, (LONG_PTR)MileNewWndProc);
		MileWindow::IsWindowCreated(true);
	}
}

/// <summary>
/// �����
/// </summary>
void MileWindow::Activate(int nShowCmd)
{
	if (MileWindow::IsWindowCreated() == true)
	{
		ShowWindow(MileWindow::Handle(), nShowCmd);
		UpdateWindow(MileWindow::Handle());

		MileWindow::SetAppIcon();
	}

	MSG Message;

	while (GetMessage(&Message, nullptr, WM_NULL, WM_NULL))
	{
		// Workaround for capturing Alt+F4 in applications with XAML Islands.
		// Reference: https://github.com/microsoft/microsoft-ui-xaml/issues/2408
		if (Message.message == WM_SYSKEYDOWN && Message.wParam == VK_F4)
		{
			PostMessage(GetAncestor(Message.hwnd, GA_ROOT), Message.message, Message.wParam, Message.lParam);
			continue;
		}

		TranslateMessage(&Message);
		DispatchMessage(&Message);
	}
}

/// <summary>
/// ����Ӧ�ô���ͼ��
/// </summary>
void MileWindow::SetAppIcon()
{
	TCHAR szFullPath[MAX_PATH];
	ZeroMemory(szFullPath, MAX_PATH);
	GetModuleFileName(NULL, szFullPath, MAX_PATH);
	HICON AppIcon = MileWindow::LoadLocalExeIcon(szFullPath);
	SendMessage(ApplicationRoot->MainWindow.Handle(), WM_SETICON, ICON_BIG, (LPARAM)AppIcon);
	SendMessage(ApplicationRoot->MainWindow.Handle(), WM_SETICON, ICON_SMALL, (LPARAM)AppIcon);
}

/// <summary>
/// ����Ӧ�ô���ͼ��
/// </summary>
HICON MileWindow::LoadLocalExeIcon(LPCWSTR exeFile)
{
	// ѡ���ļ��е�ͼ������
	int iconTotalCount = PrivateExtractIcons(exeFile, 0, 0, 0, nullptr, nullptr, 0, 0);

	// ���ڽ��ջ�ȡ����ͼ��ָ��
	HICON* hIcons = new HICON[iconTotalCount];

	// ��Ӧ��ͼ��id
	UINT* ids = new UINT[iconTotalCount];

	// �ɹ���ȡ����ͼ�����
	int successCount = PrivateExtractIcons(exeFile, 0, 16, 16, hIcons, ids, iconTotalCount, 0);

	// FileRenamer.exe Ӧ�ó���ֻ��һ��ͼ�꣬���ظ�Ӧ�ó����ͼ����
	if (successCount >= 1 && hIcons[0] != nullptr)
	{
		return hIcons[0];
	}
	else
	{
		return nullptr;
	}
}

/// <summary>
/// ������Ϣ����
/// </summary>
LRESULT CALLBACK MileNewWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch (msg)
	{
	// ���ڴ�С��������ʱ����Ϣ
	case WM_GETMINMAXINFO:
	{
		MINMAXINFO* minMaxInfo = (MINMAXINFO*)lParam;
		if (ApplicationRoot->MainWindow.MinWindowSize.X >= 0)
		{
			minMaxInfo->ptMinTrackSize.x = DPICalcHelper::ConvertEpxToPixel(ApplicationRoot->MainWindow.Handle(), ApplicationRoot->MainWindow.MinWindowSize.X);
		}
		if (ApplicationRoot->MainWindow.MinWindowSize.Y >= 0)
		{
			minMaxInfo->ptMinTrackSize.y = DPICalcHelper::ConvertEpxToPixel(ApplicationRoot->MainWindow.Handle(), ApplicationRoot->MainWindow.MinWindowSize.Y);
		}
		if (ApplicationRoot->MainWindow.MaxWindowSize.X > 0)
		{
			minMaxInfo->ptMinTrackSize.x = DPICalcHelper::ConvertEpxToPixel(ApplicationRoot->MainWindow.Handle(), ApplicationRoot->MainWindow.MaxWindowSize.X);
		}
		if (ApplicationRoot->MainWindow.MaxWindowSize.Y > 0)
		{
			minMaxInfo->ptMinTrackSize.y = DPICalcHelper::ConvertEpxToPixel(ApplicationRoot->MainWindow.Handle(), ApplicationRoot->MainWindow.MaxWindowSize.Y);
		}
	}
	}

	return CallWindowProc(MileOldWndProc, hwnd, msg, wParam, lParam);
}