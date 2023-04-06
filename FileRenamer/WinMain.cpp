#pragma once

#include <Windows.h>
#include <WinMain.h>
#include <fstream>
#include "pch.h"
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

void InitializeProgramResources();
bool CheckSingleInstance(LPCWSTR pszUniqueName);

/// <summary>
/// �ļ�����������
/// </summary>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	InitializeProgramResources();

	if (CheckSingleInstance(L"Gaoyifei1011.FileRenamer") == false)
	{
		//TODO:����ض����������
	}
	else
	{
		init_apartment(apartment_type::single_threaded);
		ApplicationRoot = make_self<implementation::App>();
		ApplicationRoot->Run(hInstance, nShowCmd);
	}

	return 0;
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
bool CheckSingleInstance(LPCWSTR pszUniqueName)
{
	HANDLE hMutex = CreateEvent(NULL, TRUE, FALSE, pszUniqueName);
	DWORD dwLstErr = GetLastError();
	bool bOneInstanceCheck = true;

	if (hMutex)
	{
		if (dwLstErr == ERROR_ALREADY_EXISTS)
		{
			CloseHandle(hMutex);
			bOneInstanceCheck = false;
		}
	}
	else
	{
		if (dwLstErr == ERROR_ACCESS_DENIED)
		{
			bOneInstanceCheck = false;
		}
	}

	return bOneInstanceCheck;
}