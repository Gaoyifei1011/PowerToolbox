#pragma once

#include <functional>
#include <winrt/base.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Input.h>

using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Xaml::Input;

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	///  һ�������Ψһ��;��ͨ������ί�н��书���м̵���������
	/// </summary>
	struct RelayCommand : winrt::implements<RelayCommand, ICommand>
	{
		RelayCommand(std::function<void(IInspectable)> action);

		void Execute(IInspectable parameter);
		bool CanExecute(IInspectable parameter);
		void CanExecuteChanged(winrt::event_token const& token) noexcept;

		winrt::event_token CanExecuteChanged(EventHandler<IInspectable> const& handler);

	private:
		std::function<void(IInspectable)> m_action;
		winrt::event<EventHandler<IInspectable>> m_eventToken;
	};
}
