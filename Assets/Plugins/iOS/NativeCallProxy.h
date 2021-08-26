// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>
#import <Metal/Metal.h>

@protocol NativeCallCommonProtocol

@required
- (void)didLoadScene:(NSString *)sceneName;
- (void)didUnLoadScene:(NSString *)sceneName;

@end

@protocol NativeCallRenderProtocol <NSObject>

@required
- (void)onRenderEvent:(int)eventID;
- (void)setupTextureFromUnity:(id<MTLTexture>)texture;

@end

@protocol NativeCallGameProtocol <NSObject>

@required
- (void)didLoadGame:(int)gameType;
- (void)didUnloadGame:(int)gameType;
- (void)updateScore:(int)score;

@end

__attribute__ ((visibility("default")))
@interface FrameworkLibAPI : NSObject
// call it any time after UnityFrameworkLoad to set object implementing NativeCallsProtocol methods
+ (void)registerNativeCallCommonApi:(id<NativeCallCommonProtocol>)api;
+ (void)registerNativeCallRenderApi:(id<NativeCallRenderProtocol>)api;
+ (void)registerNativeCallGameApi:(id<NativeCallGameProtocol>)api;
+ (void)unregisterNativeCallCommonApi;
+ (void)unregisterNativeCallRenderApi;
+ (void)unregisterNativeCallGameApi;
@end
