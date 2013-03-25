//
//  AuImage.h
//  DroneBattle2
//
//  Created by Fabrizio Polo on 2/27/12.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#ifndef com_Orbotix_FabrizioPolo_Aure_AureImage_h
#define com_Orbotix_FabrizioPolo_Aure_AureImage_h

#include "Aure.h"

#pragma mark -
#pragma mark AuImage and AuColor Ty pes

#ifdef __cplusplus
extern "C"
{
#endif

//
//          Types
//

//  Class AuImage
//typedef struct AuImageData AuImage;
struct AuImageStruct
{
    au_byte* data;              //  A pointer to the raw data.
    int bytesPerRow;
    au_byte* data2;             //  Pointer to a second image plane.
    
    int rows;
    int cols;
    int channels;
    int bytesPerSample;

    AU_ENUM format;
    AuImage* parent;            //  Keep track of an enclosing frame
    
    //  Store a reference to your native image object type here, so you can refer to it in relinquishFrame(...)
    void* nativeImageObjectRef;
    
    //  When it drops to zero, we automatically call the relinquish method
    int refCount;
    AU_ERROR (*relinquishMethod)(AuImage* img);
    au_time timestamp;          //  An optional timestamp
};

//  Class AuColor
//typedef struct AuColorData AuColor;
//struct AuColorStruct
//{
//    au_scalar R, G, B;
//};


#pragma mark -
#pragma mark AuColor Methods


//
//          AuColor Methods
//

//  Use these together as constructors.

int sizeof_AuColor(void);                       //  Returns the size in bytes of an AuColor object.  (Use this for memory allocation).                                          
AuColor* init_AuColor(void* ptr, AU_ENUM format);    //  Initialize an AuColor object at the given location.
AuColor* new_AuColor(AU_ENUM format);              //  call malloc to make a new AuColor instance.

//void AuColorSet(AuEnum colorFormat, au_byte A, au_byte B, au_byte C);
void AuColorSetBytes_RGB(AuColor* color, au_byte R, au_byte G, au_byte B);
void AuColorSet_RGB(AuColor* color, au_scalar R, au_scalar G, au_scalar B);

au_byte AuColorGetByte_R(const AuColor*const col);
au_byte AuColorGetByte_G(const AuColor*const col);
au_byte AuColorGetByte_B(const AuColor*const col);

au_scalar AuColorGet_R(const AuColor*const col);
au_scalar AuColorGet_G(const AuColor*const col);
au_scalar AuColorGet_B(const AuColor*const col);

au_scalar AuColorGetL2Distance2(const AuColor*const color1, const AuColor*const color2, const AuColor*const weights);
int AuColorIsWhite(const AuColor*const color);
//  Print a description of the color to the log
void AuColorPrint(const AuColor*const color);
void AuColorCopy(const AuColor*const thisColor, AuColor*const copy);


#pragma mark -
#pragma mark AuImage Methods

//
//          AuImage Methods
//



//  Use these together as a constructor
int sizeof_AuImage(void);
AuImage* init_AuImage(void* ptr, AU_ENUM imageFormat);
AuImage* new_AuImage(AU_ENUM imageFormat);

//  Alloate data space for an image and zero it (or not)
AuImage* AuImageAllocateData(AuImage* img, au_bool shouldZero);

//  Initializes an image header assuming no empty space between lines.
AuImage* init_TightAuImage(void* ptr, AU_ENUM imageFormat, int rows, int cols);


#pragma mark -
#pragma mark Memory Management

//
//      Memory Management
//

#ifdef __cplusplus
extern "C" {
#endif
//  Retains the image (by retaining its root) and returns the root.
AuImage* AuImageRetain(AuImage* img);       //  Returns img for convenience.
//  Release an image.  If refcount == 0, call relinquishMethod
AU_ERROR AuImageRelease(AuImage* img);
    
#ifdef __cplusplus
}
#endif


#pragma mark -
#pragma mark AuImage Utilities

//
//          AuImage Utilities
//

//  The step size from the upper bound of one row to the beginning of the next.
int AuImageGetRowDifference(const AuImage*const img);

//  Get a pointer to the beginning of a row.
au_byte* AuImageGetRowPtr(const AuImage*const img, const int row);

//  Get a pointer to the given sample
au_byte* AuImageGetSamplePtr(const AuImage*const img, const int row, const int col);

//  Compute bytes per pixel from other properties.
int AuImageGetBytesPerPixel(const AuImage*const img);

//  The total size: img->data + size is a pointer to the memory immediatley after the image.
int AuImageGetSizeInBytes(const AuImage*const img);

//  Computes the location in the image from the pointer.
void AuImageGetPositionFromPtr(const AuImage*const img, const au_byte*const ptr, int*const row, int*const col);


#ifdef __cplusplus
extern "C" {
#endif
//  Follows the chain img->parent->parent->... to the top.
AuImage* AuImageGetRoot(const AuImage*const img);
#ifdef __cplusplus
}
#endif
    
//  Zero three identically configured AuImages at the same time.
//  WARNING: Expects bytesPerRow to be divisible by 4.
void AuImageTripleZero(AuImage** imgs);

//  Print out a description of the image
void AuImageReport(const AuImage*const img);

#pragma mark Copying and Pasting


void AuImageBlit_BGRX32(AuImage*const source, AuImage*const target, int row, int col);


#pragma mark -
#pragma mark AuImage Public Methods

//
//          Public AuImage Methods
//
//  Get a sub-image (knows its part of a bigger picture)
AU_ERROR AuImageGetSubImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, AuImage*const subImage);

//  Same as get subimage but doesn't complain if you ask for something too big
AU_ERROR AuImageGetImproperSubImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, AuImage*const subImage);

//  Same as ...GetSubimage but clamps teh coordinates instead of returning an error.
//BUG
AU_ERROR AuImageGetLargestPossibleSubImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, const int minSize, AuImage*const subImage);

//  Same as above but with center and rad.
//  ASSUMES IMAGE SIZE IS DIVISIBLE BY DIVSIOR
AU_ERROR AuImageGetLargestSubImageFromRadius(const AuImage*const img, const int centerRow, const int centerCol, const int radius, const int minSize, const int divisor, AuImage*const subImage);

//  Same as above but with center and row/col radii
//  ASSUMES IMAGE SIZE IS DIVISIBLE BY DIVSIOR
AU_ERROR AuImageGetLargestSubImageFromRadii(const AuImage*const img, const int centerRowIn, const int centerColIn, const int rowRadIn, const int colRadIn, const int minSize, const int divisor, AuImage*const subImage);

//  Get a sub-image that's unaware it's contained in anything larger.
AU_ERROR AuImageGetCroppedImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, AuImage*const subImage);

//  Copy all the image attributes but leave the data pointers.
void AuImageCopyHeader(AuImage*const img, const AuImage*const fromImg);

//  Copy header information and assign rows, cols.  Leave the data pointer alone.
void AuImageInitFromHeaderAndSize(AuImage*const img, const AuImage*const fromImg, const int rows, const int cols);

//  Store a reference to your native image object type here, so you can refer to it in relinquishFrame(...)
void AuImageSetNativeImageObjectRef(AuImage*const img, const void*const ptr);
void* AuImageGetNativeImageObjectRef(const AuImage*const img);

//  Initialize an AuColor object by sampling the picture at the point.
void AuImageSampleColor(const AuImage*const img, const int row, const int col, AuColor* colorOut);

//  Samples the color at texture coordinates (x,y) where x, y are in [0,1].
void AuImageSampleColorAtTexCoords(const AuImage*const img, const au_scalar x, const au_scalar y, AuColor* colorOut);

//  True if ptr lies inside img root (i.e. the ptr is good)
au_bool AuImagePtrIsValid(const AuImage*const img, const au_byte*const ptr);

//  Log the row, col position of the ptr within the image.
void AuImageLogPtrPosition(const AuImage*const img, const au_byte*const ptr);


//  Uses AU_ASSERT to check that the ptr points to somewhere within the root of img
void AuImageAssertPtrIsValid(const AuImage*const img, const au_byte*const ptr);

//  Return a number of pixels guaranteed to exist around the edge of the image.
int AuImageGetMargin(const AuImage*const img);


#pragma mark -
#pragma mark Filters

//
//          AuImage Filters
//

AU_ERROR AuImageBlueDogFilter(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageRedDogFilter(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageColorSplitterFilter(const AuImage*const img, const int compression, const AuColor*const goodColor, AuColor*const badColor, AuImage*const imgOut);
AU_ERROR AuImageColorSplitterFilter_YUV420_PLANAR(const AuImage*const img, const int compression, const AuColor*const goodColor, AuColor*const badColor, AuImage*const imgOut);
AU_ERROR AuImageColorSplitterFilter_BGRX_32(const AuImage*const img, const int compression, const AuColor*const goodColor, AuColor*const badColor, AuImage*const imgOut);
AU_ERROR AuImageExperimentalFilter(const AuImage*const img, const int compression, AuImage*const imgOut);

AU_ERROR AuImageBlueFilter_BGRX32(const AuImage*const img, const int compression, AuImage*const imgOut);


//  Convert an image to GRAY16 based on brightness.
AU_ERROR AuImageWhiteFilter(const AuImage*const img, const int compression, AuImage*const imgOut);

//  AuImageWhiteFilter delegates to one of these methods depending on format of the image.
AU_ERROR AuImageWhiteFilter_YUV420_PLANAR(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageWhiteFilter_RGBX32(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageWhiteFilter_BGRX32(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageWhiteFilter_2VUY16(const AuImage*const img, const int compression, AuImage*const imgOut);
AU_ERROR AuImageWhiteFilter_RGB24(const AuImage*const img, const int compression, AuImage*const imgOut);

//  Perform an L1 color filter on the image to produce a new one
AU_ERROR AuImageL1ColorFilter(const AuImage*const imgIn, const AuColor*const filterColor, const int compression, AuImage*const imgOut);

//  AuImageL1ColorFilter delegates to one of these methods depending on format of the image.
AU_ERROR AuImageL1ColorFilter_RGBX32(const AuImage*const img, const AuColor*const filterColor, const int compression, AuImage*const imgOut);
AU_ERROR AuImageL1ColorFilter_BGRX32(const AuImage*const img, const AuColor*const filterColor, const int compression, AuImage*const imgOut);
AU_ERROR AuImageL1ColorFilter_2VUY16(const AuImage*const img, const AuColor*const filterColor, const int compression, AuImage*const imgOut);

AU_ERROR AuImageFast2x2Blur_GRAY16(AuImage*const img);

AU_ERROR AuImageSquareBlurFilter(const AuImage*const img, const int blurRad, AuImage*const blurredImage);

AU_ERROR AuImageSquareBlurFilter_GRAY16(const AuImage*const img, const int blurRad, AuImage*const blurredImage);

AU_ERROR AuImageSmallPyramidBlurFilter_GRAY16(const AuImage*const img, AuImage*const blurredImage);     //  Doubles the scale
AU_ERROR AuImageNormalizedSmallPyramidBlurFilter_GRAY16(const AuImage*const img, AuImage*const blurredImage);

AU_ERROR AuImageScaleBrightness_GRAY16(AuImage*const img, const int oldCeiling);

//
//          Macros
//

//  An absolute value for unsigned types.
#define UABS(A, B)                          ( (A) < (B) ? (B)-(A) : (A)-(B))
#define USHORT_ABS(A, B)        (ushort)    ( (A) < (B) ? (B)-(A) : (A)-(B))
	
#ifdef __cplusplus
}
#endif

//#define LOOP_OVER_ROWS(IMG, ROWVAR,
#endif




