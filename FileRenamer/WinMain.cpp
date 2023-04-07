#pragma once

#include <Windows.h>
#include <WinMain.h>
#include <fstream>
#include "pch.h"
#include <tchar.h>
#include "App.h"
#include "MainPage.h"
#include "Services/Root/ResourceService.h"
#include "Services/Window/NavigationService.h"

using namespace winrt;
using namespace winrt::FileRenamer;

com_ptr<implementation::App> ApplicationRoot;
ResourceService AppResourcesService;
NavigationService AppNavigationService;
StringFormatHelper AppStringFormatHelper;

void ApplicationStart(HINSTANCE hInstance, int nShowCmd);
void InitializeProgramResources();
bool IsAppLaunched(LPCWSTR pszUniqueName);

/// <summary>
/// �ļ�����������
/// </summary>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);

	InitializeProgramResources();

	// ���Ӧ���Ƿ�������״̬
	if (IsAppLaunched(L"Gaoyifei1011.FileRenamer") == true)
	{
		// ���Ӧ�ó����������������������ֱ�������ڶ���ʵ�������ر�֮ǰ��ʵ��
		if (_tcscmp(lpCmdLine, L"Restart") == 0)
		{
			ApplicationStart(hInstance, nShowCmd);
		}
		// �رյ�ǰʵ�������ض��򵽵�һ��ʵ��
		else
		{
			// �ض������
			return 0;
		}
	}
	// û�У����е�һ��ʵ��
	else
	{
		ApplicationStart(hInstance, nShowCmd);
	}

	return 0;
}

/// <summary>
/// ��ʼ��������Ӧ��ʵ��
/// </summary>
void ApplicationStart(HINSTANCE hInstance, int nShowCmd)
{
	init_apartment(apartment_type::single_threaded);
	ApplicationRoot = make_self<implementation::App>();
	ApplicationRoot->Run(hInstance, nShowCmd);
}

/// <summary>
/// ����Ӧ�ó����������Դ
/// </summary>
void InitializeProgramResources()
{
	LanguageModel defaultLanguage = make<implementation::LanguageModel>();
	LanguageModel currentLanguage = make<implementation::LanguageModel>();

	defaultLanguage.DisplayName(L"English (United States)");
	currentLanguage.DisplayName(L"���ģ����壩");
	defaultLanguage.InternalName(L"en-us");
	currentLanguage.InternalName(L"zh-hans");
	AppResourcesService.InitializeResource(defaultLanguage, currentLanguage);
}

/// <summary>
/// ���Ӧ�ó����Ƿ��Ѿ�����
/// </summary>
bool IsAppLaunched(LPCWSTR pszUniqueName)
{
	HANDLE hMutex = CreateEvent(NULL, TRUE, FALSE, pszUniqueName);
	DWORD dwLstErr = GetLastError();
	bool isAppLaunched = false;

	if (hMutex)
	{
		if (dwLstErr == ERROR_ALREADY_EXISTS)
		{
			CloseHandle(hMutex);
			isAppLaunched = true;
		}
	}
	else
	{
		if (dwLstErr == ERROR_ACCESS_DENIED)
		{
			isAppLaunched = true;
		}
	}

	return isAppLaunched;
}