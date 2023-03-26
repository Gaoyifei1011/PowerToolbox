#pragma once

#include "RelayCommand.h"

#include <functional>

namespace winrt::FileRenamer::implementation
{
	RelayCommand::RelayCommand(std::function<void(IInspectable)> action)
	{
		m_action = action;
	}

	void RelayCommand::Execute(IInspectable parameter)
	{
		if (m_action != nullptr)
		{
			m_action(parameter);
		}
	}

	/// <summary>
	/// ʹ�� CanExecute ʱҪ���õĿ�ѡ������
	/// </summary>
	bool RelayCommand::CanExecute(IInspectable parameter)
	{
		return true;
	}

	void RelayCommand::CanExecuteChanged(winrt::event_token const& token) noexcept
	{
		m_eventToken.remove(token);
	}

	winrt::event_token RelayCommand::CanExecuteChanged(EventHandler<IInspectable> const& handler)
	{
		return m_eventToken.add(handler);
	}
}