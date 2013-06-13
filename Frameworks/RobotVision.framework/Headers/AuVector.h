//
//  AuVector.h
//  RobotVision
//
//  Created by Fabrizio Polo on 3/19/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_AuVector_h
#define RobotVision_AuVector_h

#include "Aure.h"

#ifdef __cplusplus
extern "C" {
#endif

    
//  Non-T-Versions
CLASS_DEF(AuMatrix)   //  Already defined in header
CLASS_DEF(AuQuaternion)
//CLASS_DEF(AuQMatrix)

    
CLASS_BEGIN(AuQuaternion)
au_scalar w, x, y, z;
CLASS_END
    
CLASS_BEGIN(AuMatrix)
au_scalar m11, m12, m13, m14;
au_scalar m21, m22, m23, m24;
au_scalar m31, m32, m33, m34;
au_scalar m41, m42, m43, m44;
CLASS_END


CLASS_BEGIN(AuQMatrix)
AuQuaternion orient;
au_scalar x, y, z;
CLASS_END

    
#pragma mark -
#pragma mark AuQuaternion
    
    /*-------------------------------------------------------------------------------------------------*/
    //
    //      AuQuaternion
    //
    
    //  Get the inverse assuming q has length 1
    void AuQuaternionGetInverseOfUnit(const AuQuaternion* q, AuQuaternion* inverse);
    
    void AuQuaternionGetInverse(const AuQuaternion* q, AuQuaternion* inverse);
    
    void AuQuaternionMultiply(const AuQuaternion* p, const AuQuaternion* q, AuQuaternion* product);
    
    //  Apply a quaternion to a vector by conjugation.
    void AuQuaternionConjugateVector(const AuQuaternion* q, const au_scalar* xyzIn, au_scalar* xyzOut);
    
    au_scalar AuQuaternionGetLength(const AuQuaternion* q);
    
    void AuQuaternionSetToIdentity(AuQuaternion* q);
    
    void AuQuaternionNormalize(AuQuaternion* q);
    
    void AuQuaternionEstimateFrom4x4ActionByMultiplicationMatrix(AuQuaternion* q, const AuMatrix* qMultMatrix);
    
#pragma mark -
#pragma mark AuMatrix
    
    /*-------------------------------------------------------------------------------------------------*/
    //
    //      AuMatrix
    //
    
    //  Wrap an array of au_scalars as an AuMatrix
    AuMatrix* init_AuMatrixFromArray(const au_scalar data[]);
    
    //  Get a point to row-major scalar data.
    au_scalar* AuMatrixGetScalarPtr(const AuMatrix *a);
    
    void AuMatrixMultiply(const AuMatrix* a, const AuMatrix* b, AuMatrix* prod);
    
    void AuMatrixMultiplyWCopy(const AuMatrix a, const AuMatrix b, AuMatrix* prod);
    
    //  Multiply a and b and write to *prod.
#define AU_MATRIX_MULTIPLY_TO_REF(a, b, prod)                               \
prod->m11 = a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31 + a.m14 * b.m41;  \
prod->m12 = a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32 + a.m14 * b.m42;  \
prod->m13 = a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33 + a.m14 * b.m43;  \
prod->m14 = a.m11 * b.m14 + a.m12 * b.m24 + a.m13 * b.m34 + a.m14 * b.m44;  \
\
prod->m21 = a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31 + a.m24 * b.m41;  \
prod->m22 = a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32 + a.m24 * b.m42;  \
prod->m23 = a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33 + a.m24 * b.m43;  \
prod->m24 = a.m21 * b.m14 + a.m22 * b.m24 + a.m23 * b.m34 + a.m24 * b.m44;  \
\
prod->m31 = a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31 + a.m34 * b.m41;  \
prod->m32 = a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32 + a.m34 * b.m42;  \
prod->m33 = a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33 + a.m34 * b.m43;  \
prod->m34 = a.m31 * b.m14 + a.m32 * b.m24 + a.m33 * b.m34 + a.m34 * b.m44;  \
\
prod->m41 = a.m41 * b.m11 + a.m42 * b.m21 + a.m43 * b.m31 + a.m44 * b.m41;  \
prod->m42 = a.m41 * b.m12 + a.m42 * b.m22 + a.m43 * b.m32 + a.m44 * b.m42;  \
prod->m43 = a.m41 * b.m13 + a.m42 * b.m23 + a.m43 * b.m33 + a.m44 * b.m43;  \
prod->m44 = a.m41 * b.m14 + a.m42 * b.m24 + a.m43 * b.m34 + a.m44 * b.m44;
    
    //  Get the inverse of the given matrix
    //  WARNING: This function probably doesn't work because it's written for column-major matrices.
    //  WARNING: We correct this by taking a transpose, which breaks the formula for non-orthogonal matrices.
    void AuMatrixGetInverseWCopy(const AuMatrix a, AuMatrix* aInverse);
    
    //  Multiply the given 4d matrix by the 4d vector and write to output.
    void AuMatrixTimesVector4D(const AuMatrix* a, const au_scalar* v, au_scalar* out);
    
    //  Compute the transpose of the matrix
    void AuMatrixGetTranspose(const AuMatrix* a, AuMatrix* transpose);
    
    //  Fill the matrix with the given values.
    void AuMatrixSetWithArray(AuMatrix* a, const au_scalar b[]);
        
    //  Initialize to the identity
    void AuMatrixSetToIdentity(AuMatrix* a);
    
    void AuMatrixSetToZero(AuMatrix* a);

    void AuMatrixReport(const AuMatrix* m);

#pragma mark -
#pragma mark AuQMatrix
    
    /*-------------------------------------------------------------------------------------------------*/
    //
    //      AuQMatrix
    //
    
    void AuQMatrixGetAuMatrix(const AuQMatrix* qm, AuMatrix* mout);
    
    void AuQMatrixGetInverse(const AuQMatrix* qm, AuQMatrix* inverse);
    
    void AuQMatrixTimesVector(const AuQMatrix* qm, const au_scalar* xyzIn, au_scalar* xyzOut);
    
    void AuQMatrixSetToIdentity(AuQMatrix* qm);
    
    void AuQMatrixPrint(const AuQMatrix*const qm);
    
    
#pragma mark -
#pragma mark 3x3 Matrices

    /*-------------------------------------------------------------------------------------------------*/
    //
    //      3x3 matrices
    //
    
    //  These methods were borrowed from here.
    //  http://stackoverflow.com/questions/983999/simple-3x3-matrix-inverse-code-c
    
    float matrix3x3determinantOfMinor( int          theRowHeightY,
                              int          theColumnWidthX,
                              const float theMatrix [/*Y=*/3] [/*X=*/3] );

    float matrix3x3determinant( const float theMatrix [/*Y=*/3] [/*X=*/3] );
    
    int matrix3x3inverse( const float theMatrix [/*Y=*/3] [/*X=*/3],
                 float theOutput [/*Y=*/3] [/*X=*/3] );
    
    void matrix3x3multiply(const float A[9], const float B[9], float prod[9]);
    
    au_scalar matrix2x2determinant(const float* m);
    void matrix2x2inverse(const float*m, float* inverse_out);
    void matrix2x2multiply(const float* m1, const float* m2, float* prod_out);
    
#ifdef __cplusplus
}
#endif

#endif
