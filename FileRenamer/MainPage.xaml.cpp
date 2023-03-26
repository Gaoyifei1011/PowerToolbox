#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPage.g.cpp"
#include <string>
#include <WinUser.h>
#include <winrt/Windows.Foundation.Collections.h>
#include "WinMain.h"

using namespace std;
using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Input;

namespace winrt::FileRenamer::implementation
{
	MainPage::MainPage()
	{
		InitializeComponent();
		//TCHAR szFullPath[MAX_PATH];
		//ZeroMemory(szFullPath, MAX_PATH);
		//::GetModuleFileName(NULL, szFullPath, MAX_PATH);
		//HICON icon = LoadLocalExeIcon(szFullPath);
		//::SendMessage(WindowHandle, WM_SETICON, ICON_BIG, (LPARAM)icon);
		//::SendMessage(WindowHandle, WM_SETICON, ICON_SMALL, (LPARAM)icon);

		_clickCommand = winrt::make<RelayCommand>([this](IInspectable parameter)
			{
				TCHAR szFullPath[MAX_PATH];
				ZeroMemory(szFullPath, MAX_PATH);
				::GetModuleFileName(NULL, szFullPath, MAX_PATH);
				MessageBox(WindowHandle, L"���ԶԻ���", szFullPath, MB_OK);
			});
	}

	int32_t MainPage::MyProperty()
	{
		throw hresult_not_implemented();
	}

	void MainPage::MyProperty(int32_t /* value */)
	{
		throw hresult_not_implemented();
	}

	void MainPage::HyperlinkButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{
		TCHAR szFullPath[MAX_PATH];
		ZeroMemory(szFullPath, MAX_PATH);
		::GetModuleFileName(NULL, szFullPath, MAX_PATH);
		MessageBox(WindowHandle, L"���ԶԻ���", szFullPath, MB_OK);
	}

	ICommand MainPage::ClickCommand()
	{
		return _clickCommand;
	}

	/// <summary>
	/// ��exeӦ�ó����м���ͼ���ļ�
	/// </summary>
	HICON MainPage::LoadLocalExeIcon(LPCWSTR exeFile)
	{
		// ѡ���ļ��е�ͼ������
		int iconTotalCount = PrivateExtractIcons(exeFile, 0, 0, 0, nullptr, nullptr, 0, 0);

		// ���ڽ��ջ�ȡ����ͼ��ָ��
		HICON* hIcons = new HICON[iconTotalCount];

		// ��Ӧ��ͼ��id
		UINT* ids = new UINT[iconTotalCount];

		// �ɹ���ȡ����ͼ�����
		int successCount = PrivateExtractIcons(exeFile, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

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
}