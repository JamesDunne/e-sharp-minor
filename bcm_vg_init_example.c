#include <stdint.h>
#include <stdlib.h>
#include <assert.h>

// assumptions
#define VCHPRE_
#define VCHPOST_

// bcm_host.h:
//////////////////////
void bcm_host_init(void);
void bcm_host_deinit(void);

int32_t graphics_get_display_size( const uint16_t display_number,
                                                    uint32_t *width,
                                                    uint32_t *height);

unsigned bcm_host_get_peripheral_address(void);
unsigned bcm_host_get_peripheral_size(void);
unsigned bcm_host_get_sdram_address(void);

// vc_dispmanx_types.h
//////////////////////
/* Opaque handles */
typedef uint32_t DISPMANX_DISPLAY_HANDLE_T;
typedef uint32_t DISPMANX_UPDATE_HANDLE_T;
typedef uint32_t DISPMANX_ELEMENT_HANDLE_T;
typedef uint32_t DISPMANX_RESOURCE_HANDLE_T;

typedef uint32_t DISPMANX_PROTECTION_T;

#define DISPMANX_NO_HANDLE 0

#define DISPMANX_PROTECTION_MAX   0x0f
#define DISPMANX_PROTECTION_NONE  0
#define DISPMANX_PROTECTION_HDCP  11   // Derived from the WM DRM levels, 101-300

typedef enum {
  /* Bottom 2 bits sets the orientation */
  DISPMANX_NO_ROTATE = 0,
  DISPMANX_ROTATE_90 = 1,
  DISPMANX_ROTATE_180 = 2,
  DISPMANX_ROTATE_270 = 3,

  DISPMANX_FLIP_HRIZ = 1 << 16,
  DISPMANX_FLIP_VERT = 1 << 17,

  /* invert left/right images */
  DISPMANX_STEREOSCOPIC_INVERT =  1 << 19,
  /* extra flags for controlling 3d duplication behaviour */
  DISPMANX_STEREOSCOPIC_NONE   =  0 << 20,
  DISPMANX_STEREOSCOPIC_MONO   =  1 << 20,
  DISPMANX_STEREOSCOPIC_SBS    =  2 << 20,
  DISPMANX_STEREOSCOPIC_TB     =  3 << 20,
  DISPMANX_STEREOSCOPIC_MASK   = 15 << 20,

  /* extra flags for controlling snapshot behaviour */
  DISPMANX_SNAPSHOT_NO_YUV = 1 << 24,
  DISPMANX_SNAPSHOT_NO_RGB = 1 << 25,
  DISPMANX_SNAPSHOT_FILL = 1 << 26,
  DISPMANX_SNAPSHOT_SWAP_RED_BLUE = 1 << 27,
  DISPMANX_SNAPSHOT_PACK = 1 << 28
} DISPMANX_TRANSFORM_T;

typedef enum {
  /* Bottom 2 bits sets the alpha mode */
  DISPMANX_FLAGS_ALPHA_FROM_SOURCE = 0,
  DISPMANX_FLAGS_ALPHA_FIXED_ALL_PIXELS = 1,
  DISPMANX_FLAGS_ALPHA_FIXED_NON_ZERO = 2,
  DISPMANX_FLAGS_ALPHA_FIXED_EXCEED_0X07 = 3,

  DISPMANX_FLAGS_ALPHA_PREMULT = 1 << 16,
  DISPMANX_FLAGS_ALPHA_MIX = 1 << 17
} DISPMANX_FLAGS_ALPHA_T;

typedef struct {
  DISPMANX_FLAGS_ALPHA_T flags;
  uint32_t opacity;
  DISPMANX_RESOURCE_HANDLE_T mask;
} VC_DISPMANX_ALPHA_T;  /* for use with vmcs_host */

typedef enum {
  DISPMANX_FLAGS_CLAMP_NONE = 0,
  DISPMANX_FLAGS_CLAMP_LUMA_TRANSPARENT = 1,
#if __VCCOREVER__ >= 0x04000000
  DISPMANX_FLAGS_CLAMP_TRANSPARENT = 2,
  DISPMANX_FLAGS_CLAMP_REPLACE = 3
#else
  DISPMANX_FLAGS_CLAMP_CHROMA_TRANSPARENT = 2,
  DISPMANX_FLAGS_CLAMP_TRANSPARENT = 3
#endif
} DISPMANX_FLAGS_CLAMP_T;

typedef enum {
  DISPMANX_FLAGS_KEYMASK_OVERRIDE = 1,
  DISPMANX_FLAGS_KEYMASK_SMOOTH = 1 << 1,
  DISPMANX_FLAGS_KEYMASK_CR_INV = 1 << 2,
  DISPMANX_FLAGS_KEYMASK_CB_INV = 1 << 3,
  DISPMANX_FLAGS_KEYMASK_YY_INV = 1 << 4
} DISPMANX_FLAGS_KEYMASK_T;

typedef union {
  struct {
    uint8_t yy_upper;
    uint8_t yy_lower;
    uint8_t cr_upper;
    uint8_t cr_lower;
    uint8_t cb_upper;
    uint8_t cb_lower;
  } yuv;
  struct {
    uint8_t red_upper;
    uint8_t red_lower;
    uint8_t blue_upper;
    uint8_t blue_lower;
    uint8_t green_upper;
    uint8_t green_lower;
  } rgb;
} DISPMANX_CLAMP_KEYS_T;

typedef struct {
  DISPMANX_FLAGS_CLAMP_T mode;
  DISPMANX_FLAGS_KEYMASK_T key_mask;
  DISPMANX_CLAMP_KEYS_T key_value;
  uint32_t replace_value;
} DISPMANX_CLAMP_T;

// vc_image_types.h
//////////////////////
typedef struct tag_VC_RECT_T {
   int32_t x;
   int32_t y;
   int32_t width;
   int32_t height;
} VC_RECT_T;

// vc_dispmanx.h
//////////////////////
// Displays
// Opens a display on the given device
VCHPRE_ DISPMANX_DISPLAY_HANDLE_T VCHPOST_ vc_dispmanx_display_open( uint32_t device );

// Updates
// Start a new update, DISPMANX_NO_HANDLE on error
VCHPRE_ DISPMANX_UPDATE_HANDLE_T VCHPOST_ vc_dispmanx_update_start( int32_t priority );
// Add an elment to a display as part of an update
VCHPRE_ DISPMANX_ELEMENT_HANDLE_T VCHPOST_ vc_dispmanx_element_add ( DISPMANX_UPDATE_HANDLE_T update, DISPMANX_DISPLAY_HANDLE_T display,
                                                                     int32_t layer, const VC_RECT_T *dest_rect, DISPMANX_RESOURCE_HANDLE_T src,
                                                                     const VC_RECT_T *src_rect, DISPMANX_PROTECTION_T protection, 
                                                                     VC_DISPMANX_ALPHA_T *alpha,
                                                                     DISPMANX_CLAMP_T *clamp, DISPMANX_TRANSFORM_T transform );
// End an update and wait for it to complete
VCHPRE_ int VCHPOST_ vc_dispmanx_update_submit_sync( DISPMANX_UPDATE_HANDLE_T update );

// eglplatform.h:
//////////////////////
typedef int32_t khronos_int32_t;
typedef khronos_int32_t EGLint;

/* TODO: EGLNativeWindowType is really one of these but I'm leaving it
 * as void* for now, in case changing it would cause problems
 */
typedef struct {
   DISPMANX_ELEMENT_HANDLE_T element;
   int width;   /* This is necessary because dispmanx elements are not queriable. */
   int height;
} EGL_DISPMANX_WINDOW_T;

typedef void *EGLNativeDisplayType;
typedef void *EGLNativePixmapType;
typedef void *EGLNativeWindowType;

/* EGL 1.2 types, renamed for consistency in EGL 1.3 */
typedef EGLNativeDisplayType NativeDisplayType;
typedef EGLNativePixmapType  NativePixmapType;
typedef EGLNativeWindowType  NativeWindowType;

// egl.h
//////////////////////
#define EGLAPI
#define EGLAPIENTRY

/* EGL Types */
/* EGLint is defined in eglplatform.h */
typedef unsigned int EGLBoolean;
typedef unsigned int EGLenum;
typedef void *EGLConfig;
typedef void *EGLContext;
typedef void *EGLDisplay;
typedef void *EGLSurface;
typedef void *EGLClientBuffer;

/* EGL aliases */
#define EGL_FALSE       ((EGLBoolean)0)
#define EGL_TRUE        ((EGLBoolean)1)

/* Out-of-band handle values */
#define EGL_DEFAULT_DISPLAY      ((EGLNativeDisplayType)0)
#define EGL_NO_CONTEXT        ((EGLContext)0)
#define EGL_NO_DISPLAY        ((EGLDisplay)0)
#define EGL_NO_SURFACE        ((EGLSurface)0)

/* Out-of-band attribute value */
#define EGL_DONT_CARE         ((EGLint)-1)

/* Errors / GetError return values */
#define EGL_SUCCESS        0x3000
#define EGL_NOT_INITIALIZED      0x3001
#define EGL_BAD_ACCESS        0x3002
#define EGL_BAD_ALLOC         0x3003
#define EGL_BAD_ATTRIBUTE     0x3004
#define EGL_BAD_CONFIG        0x3005
#define EGL_BAD_CONTEXT       0x3006
#define EGL_BAD_CURRENT_SURFACE     0x3007
#define EGL_BAD_DISPLAY       0x3008
#define EGL_BAD_MATCH         0x3009
#define EGL_BAD_NATIVE_PIXMAP    0x300A
#define EGL_BAD_NATIVE_WINDOW    0x300B
#define EGL_BAD_PARAMETER     0x300C
#define EGL_BAD_SURFACE       0x300D
#define EGL_CONTEXT_LOST      0x300E   /* EGL 1.1 - IMG_power_management */

/* Config attributes */
#define EGL_ALPHA_SIZE        0x3021
#define EGL_BLUE_SIZE         0x3022
#define EGL_GREEN_SIZE        0x3023
#define EGL_RED_SIZE       0x3024
#define EGL_SAMPLES        0x3031
#define EGL_SURFACE_TYPE      0x3033
#define EGL_NONE        0x3038   /* Attrib list terminator */
#define EGL_LUMINANCE_SIZE    0x303D

/* Config attribute mask bits */
#define EGL_PBUFFER_BIT       0x0001   /* EGL_SURFACE_TYPE mask bits */
#define EGL_PIXMAP_BIT        0x0002   /* EGL_SURFACE_TYPE mask bits */
#define EGL_WINDOW_BIT        0x0004   /* EGL_SURFACE_TYPE mask bits */
#define EGL_VG_COLORSPACE_LINEAR_BIT   0x0020   /* EGL_SURFACE_TYPE mask bits */
#define EGL_VG_ALPHA_FORMAT_PRE_BIT 0x0040   /* EGL_SURFACE_TYPE mask bits */
#define EGL_MULTISAMPLE_RESOLVE_BOX_BIT 0x0200  /* EGL_SURFACE_TYPE mask bits */
#define EGL_SWAP_BEHAVIOR_PRESERVED_BIT 0x0400  /* EGL_SURFACE_TYPE mask bits */

#define EGL_OPENGL_ES_BIT     0x0001   /* EGL_RENDERABLE_TYPE mask bits */
#define EGL_OPENVG_BIT        0x0002   /* EGL_RENDERABLE_TYPE mask bits */
#define EGL_OPENGL_ES2_BIT    0x0004   /* EGL_RENDERABLE_TYPE mask bits */
#define EGL_OPENGL_BIT        0x0008   /* EGL_RENDERABLE_TYPE mask bits */

/* BindAPI/QueryAPI targets */
#define EGL_OPENGL_ES_API     0x30A0
#define EGL_OPENVG_API        0x30A1
#define EGL_OPENGL_API        0x30A2

/* EGL Functions */
EGLAPI EGLint EGLAPIENTRY eglGetError(void);

EGLAPI EGLDisplay EGLAPIENTRY eglGetDisplay(EGLNativeDisplayType display_id);
EGLAPI EGLBoolean EGLAPIENTRY eglInitialize(EGLDisplay dpy, EGLint *major, EGLint *minor);
EGLAPI EGLBoolean EGLAPIENTRY eglTerminate(EGLDisplay dpy);

EGLAPI EGLBoolean EGLAPIENTRY eglChooseConfig(EGLDisplay dpy, const EGLint *attrib_list,
            EGLConfig *configs, EGLint config_size,
            EGLint *num_config);

EGLAPI EGLSurface EGLAPIENTRY eglCreateWindowSurface(EGLDisplay dpy, EGLConfig config,
              EGLNativeWindowType win,
              const EGLint *attrib_list);

EGLAPI EGLBoolean EGLAPIENTRY eglBindAPI(EGLenum api);

EGLAPI EGLContext EGLAPIENTRY eglCreateContext(EGLDisplay dpy, EGLConfig config,
             EGLContext share_context,
             const EGLint *attrib_list);
EGLAPI EGLBoolean EGLAPIENTRY eglDestroyContext(EGLDisplay dpy, EGLContext ctx);
EGLAPI EGLBoolean EGLAPIENTRY eglMakeCurrent(EGLDisplay dpy, EGLSurface draw,
           EGLSurface read, EGLContext ctx);

EGLAPI EGLBoolean EGLAPIENTRY eglReleaseThread(void);

////////////////
int main() {
   uint32_t width, height;
   int s;

   EGLDisplay        egldisplay;
   EGLConfig         eglconfig;
   EGLSurface        eglsurface;
   EGLContext        eglcontext;

   static EGL_DISPMANX_WINDOW_T nativewindow;

   DISPMANX_ELEMENT_HANDLE_T dispman_element;
   DISPMANX_DISPLAY_HANDLE_T dispman_display;
   DISPMANX_UPDATE_HANDLE_T dispman_update;
   VC_RECT_T dst_rect;
   VC_RECT_T src_rect;

   bcm_host_init();

   s = graphics_get_display_size(0 /* LCD */, &width, &height);
   assert( s >= 0 );

   dst_rect.x = 0;
   dst_rect.y = 0;
   dst_rect.width = width;
   dst_rect.height = height;
      
   src_rect.x = 0;
   src_rect.y = 0;
   src_rect.width = width << 16;
   src_rect.height = height << 16;        

   dispman_display = vc_dispmanx_display_open( 0 /* LCD */);
   dispman_update = vc_dispmanx_update_start( 0 );
         
   dispman_element = vc_dispmanx_element_add ( dispman_update, dispman_display,
      1/*layer*/, &dst_rect, 0/*src*/,
      &src_rect, DISPMANX_PROTECTION_NONE, 0 /*alpha*/, 0/*clamp*/, 0/*transform*/);
      
   nativewindow.element = dispman_element;
   nativewindow.width = width;
   nativewindow.height = height;
   vc_dispmanx_update_submit_sync( dispman_update );

   // init(&nativewindow):
   NativeWindowType window = &nativewindow;
   static const EGLint s_configAttribs[] =
   {
      EGL_RED_SIZE,       8,
      EGL_GREEN_SIZE,     8,
      EGL_BLUE_SIZE,      8,
      EGL_ALPHA_SIZE,     8,
      EGL_LUMINANCE_SIZE, EGL_DONT_CARE,        //EGL_DONT_CARE
      EGL_SURFACE_TYPE,   EGL_WINDOW_BIT,
      EGL_SAMPLES,        1,
      EGL_NONE
   };
   EGLint numconfigs;

   egldisplay = eglGetDisplay(EGL_DEFAULT_DISPLAY);
   eglInitialize(egldisplay, NULL, NULL);
   assert(eglGetError() == EGL_SUCCESS);
   eglBindAPI(EGL_OPENVG_API);

   eglChooseConfig(egldisplay, s_configAttribs, &eglconfig, 1, &numconfigs);
   assert(eglGetError() == EGL_SUCCESS);
   assert(numconfigs == 1);

   eglsurface = eglCreateWindowSurface(egldisplay, eglconfig, window, NULL);
   assert(eglGetError() == EGL_SUCCESS);
   eglcontext = eglCreateContext(egldisplay, eglconfig, NULL, NULL);
   assert(eglGetError() == EGL_SUCCESS);
   eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext);
   assert(eglGetError() == EGL_SUCCESS);

   // render loop:
   while (1) {
      //render(width, height);
      //rotateN += 1.0f;
   }

   // deinit:
   eglMakeCurrent(egldisplay, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
   assert(eglGetError() == EGL_SUCCESS);
   eglTerminate(egldisplay);
   assert(eglGetError() == EGL_SUCCESS);
   eglReleaseThread();

}