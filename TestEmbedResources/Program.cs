namespace TestEmbedResources;

using DeafMan1983.Interop.SDL2;
using static DeafMan1983.Interop.SDL2.SDL;
using static DeafMan1983.Interop.SDL2.IMG;

using static DeafMan1983.ConvFunctions;

using EmbedResourceCSharp;

using System;
using System.Runtime.InteropServices;

unsafe partial class Program
{
    // Loading image from relative path
    const string FromPath = "data/testFromPath.png";

    // Loading native library from embedded image ( only FreeBSD / Linux )
    [DllImport("c")]
    private static extern void* dlopen(sbyte* so_path, int so_flag);
    [DllImport("c")]
    private static extern void* dlsym(void* so_handle, sbyte* so_procname);
    const int RTLD_LAZY = 0x0001;

    static delegate* <byte*> func_from_library;
    static delegate* <int> func_from_library_length;

    // Loading data from embedded resource via EmbedResourceCSharp
    [FileEmbed("data/TestFromData.png")]
    private static partial ReadOnlySpan<byte> GetFromData();

    static void Main()
    {
        string sdl_error_data = StringFromSBytePointer(SDL_GetError());
        string img_error_data = StringFromSBytePointer(IMG_GetError());

        if (SDL_Init(SDL_INIT_EVERYTHING) != 0)
        {
            Console.WriteLine("Error: SDL2 initialies failed, {0}", sdl_error_data );
            return;
        }

        sbyte* win_title = SBytePointerFromString("Loading images from ...");
        int win_pos = (int)SDL_WINDOWPOS_CENTERED;
        int win_w = 400;
        int win_h = 500;
        uint win_flag = (uint)SDL_WindowFlags.SDL_WINDOW_SHOWN;
        SDL_Window* window = SDL_CreateWindow(win_title, win_pos, win_pos, win_w, win_h, win_flag);
        if (window == null)
        {
            Console.WriteLine("Error: SDL_Window creates failed, {0}", sdl_error_data);
            return;
        }

        SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, (uint)SDL_RendererFlags.SDL_RENDERER_SOFTWARE);
        if (renderer == null)
        {
            Console.WriteLine("Error: SDL_Renderer creates failed, {0}", sdl_error_data);
            return;
        }

        if (SDL_SetRenderDrawColor(renderer, 255, 255 / 3, 0, 255) != 0)
        {
            Console.WriteLine("Error: Color renders wrong, {0}", sdl_error_data);
            return;
        }

        if (IMG_Init(IMG_INIT_PNG) == 0)
        {
            Console.WriteLine("Error: IMG initializes failed, {0}", img_error_data);
            return;
        }

        // Load image from path:
        SDL_Texture* tex_from_path = IMG_LoadTexture(renderer, SBytePointerFromString(FromPath));
        if (tex_from_path == null)
        {
            Console.WriteLine("Error: Loading image from path, {0}", img_error_data);
            return;
        }

        // First loading native library
        void* so_handle = dlopen(SBytePointerFromString("data/TestEmbedLibrary.so"), RTLD_LAZY);
        if (so_handle == null)
        {
            Console.WriteLine("Error: Loading TestEmbedLibrary.so");
            return;
        }

        // data from native library
        func_from_library = (delegate* <byte*>)dlsym(so_handle, SBytePointerFromString("FromLibrary"));
        if (func_from_library == null)
        {
            Console.WriteLine("TestEmbedLibrary missed public method.");
            return;
        }

        // data size from native library
        func_from_library_length = (delegate* <int>)dlsym(so_handle, SBytePointerFromString("FromLibraryLength"));
        if (func_from_library_length == null)
        {
            Console.WriteLine("TestEmbedLibrary missed public method.");
            return;
        }
        
        // Load image from native library ( shared )
        SDL_RWops *rw_nl = SDL_RWFromMem(func_from_library(), func_from_library_length());
        SDL_Texture* tex_from_nl = IMG_LoadTexture_RW(renderer, rw_nl, 1);
        if (tex_from_nl == null)
        {
            Console.WriteLine("Error: Loading image from nativelibrary, {0}", img_error_data);
            return;
        }
        
        // Load image from data ( embedding resource )
        SDL_RWops* rw_dt;
        var data_span_bytes = GetFromData();
        fixed (byte* data_bytes = data_span_bytes)
        {
            rw_dt = SDL_RWFromConstMem(data_bytes, data_span_bytes.ToArray().Length);
        }

        SDL_Texture* tex_from_data = IMG_LoadTexture_RW(renderer, rw_dt, 1);
        if (tex_from_data == null)
        {
            Console.WriteLine("Error: Loading image from data, {0}", img_error_data);
            return;
        }

        SDL_Event evt;

        SDL_Rect rect_from_path = new SDL_Rect{x = 100, y = 50, w = 200, h = 100};
        SDL_Rect rect_from_library = new SDL_Rect{x = 100, y = 200, w = 200, h = 100};
        SDL_Rect rect_from_data = new SDL_Rect{x = 100, y = 350, w = 200, h = 100};

        while (true)
        {
            SDL_PollEvent(&evt);

            if (evt.type == (uint)SDL_EventType.SDL_QUIT)
            {
                break;
            }

            if (evt.type == (uint)SDL_EventType.SDL_KEYDOWN)
            {
                if (evt.key.keysym.sym == SDL_KeyCode.SDLK_ESCAPE)
                {
                    break;
                }
            }

            SDL_RenderClear(renderer);

            // Posit with image from path
            SDL_RenderCopy(renderer, tex_from_path, null, &rect_from_path);

            // Posit with image from native library
            SDL_RenderCopy(renderer, tex_from_nl, null, &rect_from_library);

            // Posit with image from data ( embedding resource )
            SDL_RenderCopy(renderer, tex_from_data, null, &rect_from_data);

            SDL_RenderPresent(renderer);
        }

        SDL_DestroyTexture(tex_from_path);
        SDL_DestroyTexture(tex_from_nl);
        SDL_DestroyTexture(tex_from_data);

        SDL_DestroyRenderer(renderer);
        SDL_DestroyWindow(window);

        IMG_Quit();
        SDL_Quit();
    }
}
