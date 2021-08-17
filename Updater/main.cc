#include <iostream>
#include <string>
#include <filesystem>
#include <Windows.h>

void startPorcess(LPSTR exePath)
{
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));

	CreateProcessA(
		NULL,
		exePath,
		NULL,
		NULL,
		FALSE,
		0,
		NULL,
		NULL,
		&si,
		&pi
	);

	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
}

int main(int argc, char* argv[])
{
	FreeConsole();
	if (argc < 4) return -1;

	std::string exPackPath = argv[1];
	std::string installPath = argv[2];
	std::string exeName = argv[3];

	if (!std::filesystem::exists(exPackPath)) return -1;
	if (std::filesystem::exists(installPath))
	{
		Sleep(2000);
		std::error_code ec;
		if (std::filesystem::remove_all(installPath, ec) == -1) return -1;
	}

	std::filesystem::copy(exPackPath, installPath);
	
	std::string exePath = "\"" + installPath + "\\" + exeName + "\"";
	startPorcess((LPSTR)exePath.c_str());

	return 0;
}