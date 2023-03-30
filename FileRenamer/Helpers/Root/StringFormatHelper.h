#pragma once

#include <string>
#include <memory>

using namespace std;

/// <summary>
/// ���ַ������и�ʽ�����������
/// </summary>
class StringFormatHelper
{
public:
	template<typename ... Args>
	string format(const string& format, Args ... args);

	template<typename ... Args>
	wstring format(const wstring& format, Args ... args);
};
