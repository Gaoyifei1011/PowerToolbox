#pragma once

#include <functional>
#include <winrt/base.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Input.h>

namespace winrt
{
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
}

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	///  һ�������Ψһ��;��ͨ������ί�н��书���м̵���������
	/// </summary>
	struct RelayCommand : winrt::implements<winrt::FileRenamer::implementation::RelayCommand, winrt::WinrtInput::ICommand>
	{
		RelayCommand(std::function<void(winrt::WinrtFoundation::IInspectable)> action);

		void Execute(winrt::WinrtFoundation::IInspectable parameter);
		bool CanExecute(winrt::WinrtFoundation::IInspectable parameter);
		void CanExecuteChanged(winrt::event_token const& token) noexcept;

		winrt::event_token CanExecuteChanged(winrt::WinrtFoundation::EventHandler<winrt::WinrtFoundation::IInspectable> const& handler);

	private:
		std::function<void(winrt::WinrtFoundation::IInspectable)> m_action;
		winrt::event<winrt::WinrtFoundation::EventHandler<winrt::WinrtFoundation::IInspectable>> m_eventToken;
	};
}
