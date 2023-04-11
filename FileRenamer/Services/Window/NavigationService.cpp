#include "NavigationService.h"

NavigationService::NavigationService() {};

winrt::WinrtControls::Frame NavigationService::NavigationFrame()
{
	return _navigationFrame;
}

void NavigationService::NavigationFrame(winrt::WinrtControls::Frame value)
{
	_navigationFrame = value;
}

winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> NavigationService::NavigationItemList()
{
	return _navigationItemList;
}

void NavigationService::NavigationItemList(winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> const& value)
{
	_navigationItemList = value;
}

/// <summary>
 /// ҳ����ǰ����
 /// </summary>
void NavigationService::NavigateTo(winrt::WinrtInterop::TypeName navigationPageType, winrt::WinrtFoundation::IInspectable parameter)
{
	for (uint32_t index = 0; index < NavigationService::NavigationItemList().Size(); index++)
	{
		if (NavigationService::NavigationItemList().GetAt(index).NavigationPage() == navigationPageType)
		{
			winrt::WinrtAnimation::SlideNavigationTransitionInfo info;
			info.Effect(winrt::WinrtAnimation::SlideNavigationTransitionEffect::FromRight);
			NavigationService::NavigationFrame().Navigate(
				NavigationService::NavigationItemList().GetAt(index).NavigationPage(),
				parameter,
				info
			);
		}
	}
}

/// <summary>
/// ҳ����󵼺�
/// </summary>
void NavigationService::NavigationFrom()
{
	if (NavigationService::NavigationFrame().CanGoBack())
	{
		NavigationService::NavigationFrame().GoBack();
	}
}

/// <summary>
/// ��ȡ��ǰ��������ҳ
/// </summary>
winrt::WinrtInterop::TypeName NavigationService::GetCurrentPageType()
{
	return NavigationService::NavigationFrame().CurrentSourcePageType();
}

/// <summary>
/// ��鵱ǰҳ���Ƿ�����󵼺�
/// </summary>
bool NavigationService::CanGoBack()
{
	return NavigationService::NavigationFrame().CanGoBack();
}