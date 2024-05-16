//
//  MSDKDnsUnityManager.h
//  MSDKDns
//
//  Created by fu chunhui on 16/7/25.
//  Copyright © 2016年 Tencent. All rights reserved.
//

#ifndef MSDKDnsUnityManager_h
#define MSDKDnsUnityManager_h

#import <MSDKDns/MSDKDns.h>
#define UnityReceiverObject "[Starter]"

@interface MSDKDnsUnityManager : NSObject 

- (NSString *)GetHostByName:(const char *) domain;
+ (id) sharedInstance;

@end

#endif /* MSDKDnsUnityManager_h */
