#!/bin/bash
if [ $# -eq 0 ]; then
    echo "Must provide version number as an argument"
    exit 1
fi

./linux_build.sh

RELEASE=pcty-v$1
RELEASE_DIR=./$RELEASE
RELEASE_ARCHIVE=$RELEASE.tgz

if [ -d $RELEASE_DIR ]
then
  rm -rf $RELEASE_DIR
fi

if [ -f $RELEASE_ARCHIVE ]
then
  rm $RELEASE_ARCHIVE
fi

mkdir $RELEASE_DIR
tar -C $RELEASE_DIR -xf ./pcty_current.tgz
tar -czvf $RELEASE_ARCHIVE $RELEASE_DIR
