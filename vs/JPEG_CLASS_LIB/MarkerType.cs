using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
	enum MarkerType
	{
		//enum for Start Of Frame markers, non-differential, Huffman coding
		BaseLineDCT = 0xFFC0, 
        ExtendedSequentialDCT = 0xFFC1,
		ProgressiveDCT = 0xFFC2, 
        LosslessHuffman = 0xFFC3,

		//enum for Start Of Frame markers, differential, Huffman coding
		DiferentialSequentialDCTHuffman = 0xFFC5,
        DifferentialProgressiveDCTHuffman = 0xFFC6,
        DifferentialLoslessHuffman = 0xFFC7,

		//enum for Start Of Frame markers, non-differential, arithmetic coding
        ReservedForJPEGExt = 0xFFC8,
        ExtendedSeqDCT = 0xFFC9,
        ProgressiveDCTArithmetic = 0xFFCA,
        LosslessArithmetic = 0xFFCB,

        //enum for Start Of Frame markers, differential, arithmetic coding
        DifferentialSequentialDCTArithmetic = 0xFFCD,
        DifferentialProgressiveDCTArithmetic = 0xFFCE,
        DifferentialLoslessArithmetic = 0xFFCF,

		//enum for Huffman table specification
        DefineHuffmanTables = 0xFFC4,

		//enum for Arithmetic coding conditioning specification
        DefineArithmeticCodingConditionings = 0xFFCC,

		//enum for Restart interval termination
        RestartWithModEightCount0 = 0xFFD0,
        RestartWithModEightCount1 = 0xFFD1,
        RestartWithModEightCount2 = 0xFFD2,
        RestartWithModEightCount3 = 0xFFD3,
        RestartWithModEightCount4 = 0xFFD4,
        RestartWithModEightCount5 = 0xFFD5,
        RestartWithModEightCount6 = 0xFFD6,
        RestartWithModEightCount7 = 0xFFD7,

        //enum for Other markers
        StartOfImage = 0xFFD8,
        EndOfImage = 0xFFD9,
        StartOfScan = 0xFFDA,
        DefineQuantizationTables = 0xFFDB,
        DefineNumberOfLines = 0xFFDC,
        DefineRestartInterval = 0xFFDD,
        DefineHierarchicalProgression = 0xFFDE,
        ExpandReferenceComponents = 0xFFDF,
        ReservedForApplicationSegments = 0xFFE0,
        ReservedForJPEGExtentions = 0xFFF0,
        Comment = 0xFFFE,

		//enum for Reserved markers
        ForTemporaryPrivateUseInArithmeticCoding = 0xFF01,
        Reserved = 0xFF02
	}
}
