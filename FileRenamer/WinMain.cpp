#pragma once

#include <Windows.h>
#include <WinMain.h>
#include <fstream>
#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"
#include "Services/Root/ResourceService.h"
#include "Services/Window/NavigationService.h"

using namespace winrt;
using namespace winrt::FileRenamer;

com_ptr<implementation::App> ApplicationRoot;
ResourceService AppResourcesService;
NavigationService AppNavigationService;
StringFormatHelper AppStringFormatHelper;

void InitializeProgramResources();

/// <summary>
/// �ļ�����������
/// </summary>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	InitializeProgramResources();

	init_apartment(apartment_type::single_threaded);
	ApplicationRoot = make_self<implementation::App>();
	ApplicationRoot->Run(hInstance, nShowCmd);
	return 0;
}

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