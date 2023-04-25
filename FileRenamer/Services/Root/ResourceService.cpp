#include "LanguageModel.g.h"
#include "ResourceService.h"

ResourceService::ResourceService()
{
	_isInitialized = false;
	_resourceMap = winrt::WinrtResourcesCore::ResourceManager::Current().MainResourceMap();
}

bool ResourceService::IsInitialized()
{
	return _isInitialized;
}
void ResourceService::IsInitialized(bool value)
{
	_isInitialized = value;
}

winrt::FileRenamer::LanguageModel ResourceService::DefaultAppLanguage()
{
	return _defaultAppLanguage;
}
void ResourceService::DefaultAppLanguage(winrt::FileRenamer::LanguageModel value)
{
	_defaultAppLanguage = value;
}

winrt::FileRenamer::LanguageModel ResourceService::CurrentAppLanguage()
{
	return _currentAppLanguage;
}
void ResourceService::CurrentAppLanguage(winrt::FileRenamer::LanguageModel value)
{
	_currentAppLanguage = value;
}

winrt::WinrtResourcesCore::ResourceContext ResourceService::DefaultResourceContext()
{
	return _defaultResourceContext;
}
void ResourceService::DefaultResourceContext(winrt::WinrtResourcesCore::ResourceContext value)
{
	_defaultResourceContext = value;
}

winrt::WinrtResourcesCore::ResourceContext ResourceService::CurrentResourceContext()
{
	return _currentResourceContext;
}
void ResourceService::CurrentResourceContext(winrt::WinrtResourcesCore::ResourceContext value)
{
	_currentResourceContext = value;
}

winrt::WinrtResourcesCore::ResourceMap ResourceService::ResourceMap()
{
	return _resourceMap;
}

winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> ResourceService::ThemeList()
{
	return _themeList;
}

/// <summary>
/// ��ʼ��Ӧ�ñ��ػ���Դ
/// </summary>
/// <param name="defaultAppLanguage">Ĭ����������</param>
/// <param name="currentAppLanguage">��ǰ��������</param>
void ResourceService::InitializeResource(winrt::FileRenamer::LanguageModel defaultAppLanguage, winrt::FileRenamer::LanguageModel currentAppLanguage)
{
	ResourceService::DefaultAppLanguage(defaultAppLanguage);
	ResourceService::CurrentAppLanguage(currentAppLanguage);
	ResourceService::DefaultResourceContext().QualifierValues().Insert(L"Language", ResourceService::DefaultAppLanguage().InternalName());
	ResourceService::CurrentResourceContext().QualifierValues().Insert(L"Language", ResourceService::CurrentAppLanguage().InternalName());

	IsInitialized(true);
}

/// <summary>
/// ��ʼ��Ӧ�ñ��ػ���Ϣ
/// </summary>
void ResourceService::LocalizeResource()
{
	InitializeThemeList();
}

/// <summary>
/// ��ʼ��Ӧ��������Ϣ�б�
/// </summary>
void ResourceService::InitializeThemeList()
{
	ResourceService::ThemeList().Append(winrt::make<winrt::FileRenamer::implementation::ThemeModel>(ResourceService::GetLocalized(L"Settings/ThemeDefault"), L"Default"));
	ResourceService::ThemeList().Append(winrt::make<winrt::FileRenamer::implementation::ThemeModel>(ResourceService::GetLocalized(L"Settings/ThemeLight"), L"Light"));
	ResourceService::ThemeList().Append(winrt::make<winrt::FileRenamer::implementation::ThemeModel>(ResourceService::GetLocalized(L"Settings/ThemeDark"), L"Dark"));
}

/// <summary>
/// �ַ������ػ�
/// </summary>
winrt::hstring ResourceService::GetLocalized(winrt::hstring resource)
{
	if (ResourceService::IsInitialized())
	{
		try
		{
			return ResourceService::ResourceMap().GetValue(resource, ResourceService::CurrentResourceContext()).ValueAsString();
		}
		catch (const std::exception&)
		{
			try
			{
				return ResourceService::ResourceMap().GetValue(resource, ResourceService::DefaultResourceContext()).ValueAsString();
			}
			catch (const std::exception&)
			{
				return resource;
			}
		}
	}
	else
	{
		throw "ResourcesInitializeFailed";
	}
}