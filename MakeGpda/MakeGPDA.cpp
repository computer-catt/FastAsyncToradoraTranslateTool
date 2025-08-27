#include <iostream>
#include <fstream>
#include <vector>
#include <string>

using namespace std;

struct EntryStruct
{
	unsigned _int32 EntryOffset;
	int EntrySize;
	char* Data;
	int TotalEntrySize;
};

struct FileNameStruct
{
	string FileName;
	string InternalName;
	int NameSize;
	int RelOffset;
};

int CalculateBuffer(unsigned _int32);
void WriteBuffer(ofstream&, int);

int main(int argc, const char* argv[])
{
	
	if (argc != 2)
	{
		cout << "usage: makeGPDA targetfile\n";
		return -1;
	}

	string TargetName = argv[1];
	string TargetList = TargetName + ".lst";
	string TargetFile = TargetName + ".dat";

	ifstream InFile(TargetList.c_str() );

	if (!InFile)
	{
		cout << "Unable to open list file " << TargetList << endl;
		return -1;
	}

	vector<FileNameStruct> FileNames;
	vector<EntryStruct> Entries;
	int TotalEntries = 0;
	int EntryNameOffset = 0;

	//I'm tired of guessing if the file names need the delimiter, so I'll just toggle it in the lst file
	bool Delim = false;
	char InChar;
	InFile.get(InChar);
	InFile.ignore(1, '\n');

	if (InChar == 'Y')
		Delim = true;

	//Get list of files to compress from target list
	while(InFile)
	{
		// .lst file contains "filename on my HDD"\t"filename to store inside GPDA"\n
		string InName, InternalName;
		getline(InFile, InName, '\t'); 
		getline(InFile, InternalName, '\n'); 
		
		int InNameSize = InternalName.size();

		if (Delim)
			InNameSize++;
			
		FileNameStruct TempName = {InName, InternalName, InNameSize, EntryNameOffset};
		FileNames.push_back(TempName);

		TotalEntries++;

		InFile.peek();

		EntryNameOffset += InNameSize + 4;
	}

    InFile.close();

	unsigned _int32 TotalFileSize = 0;

	//Read all of the input files into memory
	for(int x=0; x<TotalEntries; x++)
	{
		int CurrentEntrySize = 0;
		
		string CurrentFile = FileNames[x].FileName;

		InFile.open(CurrentFile.c_str(), ios::in | ios::binary );

		if (!InFile)
		{
			cout << "Unable to open input file " << CurrentFile << endl;
			return -1;
		}

		InFile.seekg(0, std::ios::end);
		CurrentEntrySize = InFile.tellg();

		// Add the blank buffer? gotta figure out its size first
		// Looks like another buffer divisible by 0x800
		int BufferSize = CalculateBuffer(CurrentEntrySize);
	
		InFile.seekg(0, std::ios::beg);

		// Create buffer for the input file
		int TotalEntrySize = CurrentEntrySize + BufferSize;
		char* Buffer;
		Buffer = new char[TotalEntrySize];

		// Init whole buffer with 0x00
		for(int x=0; x<TotalEntrySize; x++)
			Buffer[x] = 0x00;
		
		InFile.read(Buffer, CurrentEntrySize);
		
		EntryStruct TempEntry = {TotalFileSize, CurrentEntrySize, Buffer, TotalEntrySize};
		Entries.push_back(TempEntry);

		InFile.close();

		TotalFileSize += TotalEntrySize;
	}

	//Now to go back and make the header

	// Calculate lenth of entry header
	int EntryHeader = TotalEntries * 16;

	int TotalHeaderSize = EntryHeader + EntryNameOffset + 16;

	// Add the blank buffer? gotta figure out its size first
	// The original GPDAs data started at 0x800, 0x1000, 0x2000 or 0x2800
	// All divisible by 0x800
	int BufferSize = CalculateBuffer(TotalHeaderSize);
	
	TotalFileSize += (TotalHeaderSize + BufferSize);

	// There appears to be a buffer and the end of the file as well
	int EndBufferSize = CalculateBuffer(TotalFileSize);
	
	TotalFileSize += EndBufferSize;
		
	ofstream OutFile(TargetFile.c_str(), ios::out | ios::binary | ios::trunc);

	if(!OutFile)
	{
		cout << "Unable to write to outfile file " << TargetFile << endl;
		return -1;
	}

	// Start writing header
	// 0-3 GPDA
	// 4-B ArchiveSize
	// C-F TotalEntries
	OutFile.write("GPDA", 4);
	OutFile.write(reinterpret_cast<char*>(&TotalFileSize), 8);
	OutFile.write(reinterpret_cast<char*>(&TotalEntries), 4);

	// Write entry header
	int AddOffset = 16 + EntryHeader;

	for(int x=0; x<TotalEntries; x++)
	{
		EntryStruct CurrentEntry = Entries[x];
		FileNameStruct CurrentName = FileNames[x];

		// Add relative offsets
		unsigned _int32 CorrectedOffset = CurrentEntry.EntryOffset + EntryHeader + EntryNameOffset + BufferSize + 16;
		int CurrentFileNameOffset = CurrentName.RelOffset + AddOffset;

		OutFile.write(reinterpret_cast<char*>(&CorrectedOffset), 8);
		OutFile.write(reinterpret_cast<char*>(&CurrentEntry.EntrySize), 4);
		OutFile.write(reinterpret_cast<char*>(&CurrentFileNameOffset), 4);
	}

	// Write file name header
	for(int x=0; x<TotalEntries; x++)
	{
		FileNameStruct CurrentName = FileNames[x];
		
		int TempSize = CurrentName.NameSize;

		OutFile.write(reinterpret_cast<char*>(&TempSize), 4);
		
		if (!Delim)
			OutFile.write(CurrentName.InternalName.c_str(), TempSize);
		else
		{
			OutFile.write(CurrentName.InternalName.c_str(), TempSize-1);
			char Buff[1] = {0x20};
			OutFile.write(Buff, 1);
		}
	}

	WriteBuffer(OutFile, BufferSize);

	// Output file buffers
	for(int x=0; x<TotalEntries; x++)
	{
		EntryStruct CurrentEntry = Entries[x];
		OutFile.write(CurrentEntry.Data, CurrentEntry.TotalEntrySize);
		delete [] CurrentEntry.Data;
	}

	WriteBuffer(OutFile, EndBufferSize);

	InFile.close();
	OutFile.close();

	return 0;
}

int CalculateBuffer(unsigned _int32 InputSize)
{
	int Mod8 = InputSize % 0x800;
	if (Mod8 != 0)
		return (0x800 - Mod8);
	else
		return 0;
}

void WriteBuffer(ofstream& OutFile, int Size)
{
	for(int x=0; x<Size; x++)
	{
		char Buff[1] = {0x00};
		OutFile.write(Buff, 1);
	}
}