#include "RelayCommand.h"

namespace winrt::FileRenamer::implementation
{
	RelayCommand::RelayCommand(std::function<void(winrt::WinrtFoundation::IInspectable)> action)
	{
		m_action = action;
	}

	void RelayCommand::Execute(winrt::WinrtFoundation::IInspectable parameter)
	{
		if (m_action != nullptr)
		{
			m_action(parameter);
		}
	}

	/// <summary>
	/// ʹ�� CanExecute ʱҪ���õĿ�ѡ������
	/// </summary>
	bool RelayCommand::CanExecute(winrt::WinrtFoundation::IInspectable parameter)
	{
		return true;
	}

	void RelayCommand::CanExecuteChanged(winrt::event_token const& token) noexcept
	{
		m_eventToken.remove(token);
	}

	winrt::event_token RelayCommand::CanExecuteChanged(winrt::WinrtFoundation::EventHandler<winrt::WinrtFoundation::IInspectable> const& handler)
	{
		return m_eventToken.add(handler);
	}
}