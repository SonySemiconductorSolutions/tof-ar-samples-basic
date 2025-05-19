/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */
 
#ifdef __cplusplus
extern "C" {
#endif
    
    void OpenFileAppWith(const char* path) {
        NSString *filePath = [NSString stringWithUTF8String:path];
        NSString *urlString = [@"shareddocuments://" stringByAppendingString:filePath];
        NSURL *url = [NSURL URLWithString:urlString];
        NSLog(@"%@", url);
        if ([[UIApplication sharedApplication] canOpenURL:url]) {
            [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        } else {
            NSLog(@"Cannot open the specified file path: %@", urlString);
        }
    }
    
#ifdef __cplusplus
}
#endif
