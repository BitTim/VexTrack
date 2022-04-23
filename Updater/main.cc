#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <string>
#include <filesystem>
#include <fstream>
#include <Windows.h>
#include <chrono>
#include <ctime>
#include <iomanip>
#include <sstream>

std::fstream logFile;
bool legacyMode = false;
const std::string logPath = "updateLog.txt";

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

void initLog()
{
	logFile.open(logPath, std::ios::out);
	logFile.close();
}

void log(std::string tag, std::string message)
{
	logFile.open(logPath, std::ios::app);

	char buffer[256];
	auto timeNow = std::chrono::system_clock::now();
	std::time_t now = std::chrono::system_clock::to_time_t(timeNow);

	std::stringstream ss;
	ss << std::put_time(std::localtime(&now), "%d.%m.%Y %H:%M:%S");

	sprintf_s(buffer, "[%s] - (%s): %s\n", ss.str().c_str(), tag.c_str(), message.c_str());

	logFile.write(buffer, strlen(buffer));
	logFile.close();
}

int main(int argc, char* argv[])
{
	log("Initialization", "Started");

	FreeConsole();
	if (argc < 4)
	{
		log("Arguments", "Not enough arguments");
		return -1;
	}

	std::string exPackPath = argv[1];
	std::string installPath = argv[2];
	std::string fileListPath = argv[3];
	std::string exeName;

	log("Arguments", "Assigned arguments to variables");

	if (argc >= 5) exeName = argv[4];
	else
	{
		log("Arguments", "Running in Legacy mode");
		legacyMode = true;
		exeName = fileListPath;
		fileListPath = "";
	}


	std::vector<std::string> fileList;

	if (!std::filesystem::exists(exPackPath))
	{
		log("File Checks", "Extracted files do not exist");
		return -1;
	}
	if (!std::filesystem::exists(fileListPath) && !legacyMode)
	{
		log("File Checks", "File list does not exist");
		return -1;
	}

	if(!legacyMode)
	{
		std::fstream fileListFile(fileListPath, std::ios::in);
		if (fileListFile.is_open())
		{
			log("File List Reading", "Started Reading");

			std::string line;
			while (std::getline(fileListFile, line))
			{
				fileList.push_back(line);
				log("File List Reading", line.c_str());
			}
			fileListFile.close();

			log("File List Reading", "Concluded Reading");
		}
	}
	else log("File List Reading", "Skipping due to legacy mode");
	

	if (std::filesystem::exists(installPath))
	{
		Sleep(2000);
		std::error_code ec;

		log("Installation", "Started Deletion");

		if (legacyMode)
		{
			if (std::filesystem::remove_all(installPath, ec) == -1) return -1;
			log("Installation", "Deleted everything in installation folder due to legacy mode");
		}
		else
		{
			for (std::string file : fileList)
			{
				if (!std::filesystem::remove(installPath + "\\" + file, ec))
				{
					log("Installation", ("Deletion failed for file: " + file).c_str());
					return -1;
				}
				log("Installation", (installPath + "\\" + file).c_str());
			}
		}

		log("Installation", "Concluded Deletion");
	}

	log("Installation", "Started Copying");

	if (legacyMode)
	{
		std::filesystem::copy(exPackPath, installPath);
		log("Installation", "Copying everything in extracted folder due to legacy mode");
	}
	else
	{
		for (std::string file : fileList)
		{
			if (std::filesystem::exists(exPackPath + "\\" + file))
			{
				std::filesystem::copy(exPackPath + "\\" + file, installPath + "\\" + file);
				log("Installation", (exPackPath + "\\" + file + " => " + installPath + "\\" + file).c_str());
			}
			else
			{
				log("Installation", (exPackPath + "\\" + file + " not found, skipping...").c_str());
			}
		}
	}
	

	log("Installation", "Consluded Copying");
	log("Finalization", "Creating exe path");
	
	std::string exePath = "\"" + installPath + "\\" + exeName + "\"";

	log("Finalization", exePath.c_str());
	log("Finalization", "Staring exe");

	startPorcess((LPSTR)exePath.c_str());

	log("Finalization", "Closing");
	return 0;
}