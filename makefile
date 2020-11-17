all:
	dmcs -out:/tmp/1/2/3/T.exe -r:System.Data.dll test/test_jpeg/Program.cs vs/JPEG_CLASS_LIB/*.cs
