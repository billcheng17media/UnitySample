#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"
#import "IUnityGraphics.h"

@interface FrameworkLibAPI()

@end

@implementation FrameworkLibAPI

id<NativeCallCommonProtocol> commonProtocol = nil;
id<NativeCallRenderProtocol> renderProtocol = nil;
id<NativeCallGameProtocol> gameProtocol = nil;

+ (void)unregisterNativeCallCommonApi {
    commonProtocol = nil;
}

+ (void)unregisterNativeCallRenderApi {
    renderProtocol = nil;
}

+ (void)unregisterNativeCallGameApi {
    gameProtocol = nil;
}

+ (void)registerNativeCallCommonApi:(id<NativeCallCommonProtocol>)api {
    commonProtocol = api;
}

+ (void)registerNativeCallRenderApi:(id<NativeCallRenderProtocol>)api {
    renderProtocol = api;
}

+ (void)registerNativeCallGameApi:(id<NativeCallGameProtocol>)api {
    gameProtocol = api;
}

@end

static void UNITY_INTERFACE_API OnRenderEvent(int eventID) {
    return [renderProtocol onRenderEvent:eventID];
}

extern "C" void SetRenderTextureFromUnity(void *texturePtr) {
    id<MTLTexture> tex = (__bridge id<MTLTexture>)texturePtr;
    return [renderProtocol setupTextureFromUnity:tex];
}

extern "C" void UpdateScore(int score) {
    return [gameProtocol updateScore:score];
}

extern "C" void DidLoadScene(const char *sceneName) {
    return [commonProtocol didLoadScene:[NSString stringWithUTF8String:sceneName]];
}

extern "C" void DidUnloadScene(const char *sceneName) {
    return [commonProtocol didUnLoadScene:[NSString stringWithUTF8String:sceneName]];
}

extern "C" void DidLoadGame(int gameType) {
    return [gameProtocol didLoadGame:gameType];
}

extern "C" void DidUnloadGame(int gameType) {
    return [gameProtocol didUnloadGame:gameType];
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc() {
    return OnRenderEvent;
}
