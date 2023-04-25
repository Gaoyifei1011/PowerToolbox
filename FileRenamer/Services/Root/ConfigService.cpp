#include "ConfigService.h"

ConfigService::ConfigService() {}

/// <summary>
/// ��ȡ����ѡ��洢��Ϣ
/// </summary>
winrt::WinrtFoundation::IInspectable ConfigService::ReadSettings(winrt::hstring key)
{
	if (winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().TryLookup(key) == nullptr)
	{
		return nullptr;
	}

	return winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().Lookup(key);
}

/// <summary>
/// ��������ѡ��洢��Ϣ
/// </summary>
void ConfigService::SaveSettings(winrt::hstring key, winrt::WinrtFoundation::IInspectable value)
{
	winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().Insert(key, value);
}