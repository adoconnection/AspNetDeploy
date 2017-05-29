import * as t from './actionTypes';

export const prepareLoading = () => ({
    type: t.PREPARE_LOADING
});

export const prepareLoadingVersions = (id) => ({
    type: t.PREPARE_LOADING_VERSIONS,
    payload: id
});

export const loaded = (source) => ({
    type: t.LOADED,
    payload: source
});

export const loadedVersions = (sourceVersions) => ({
    type: t.LOADED_VERSIONS,
    payload: sourceVersions
});