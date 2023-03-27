#pragma once

#include <Mile.Xaml.h>

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

using namespace winrt;
using namespace FileRenamer;
using namespace FileRenamer::implementation;

namespace winrt::FileRenamer::implementation
{
	App::App()
	{
		MileXamlGlobalInitialize();
		InitializeComponent();
	}

	/// <summary>
	/// ����Ӧ��
	/// </summary>
	void App::Run(HINSTANCE hInstance, int nShowCmd)
	{
		App::MainWindow.Content(make<MainPage>());
		App::MainWindow.Position = { CW_USEDEFAULT,0 };
		App::MainWindow.Size = { CW_USEDEFAULT,0 };
		App::MainWindow.MinWindowSize = { 600,768 };
		App::MainWindow.MaxWindowSize = { 0,0 };
		App::MainWindow.InitializeWindow(hInstance);
		App::MainWindow.Activate(nShowCmd);
	}

	/// <summary>
	/// �ر�Ӧ�ò��ͷ�������Դ
	/// </summary>
	void App::CloseApp()
	{
		MileXamlGlobalUninitialize();
		Exit();
	}
}