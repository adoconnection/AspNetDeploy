import * as t from './actionTypes';

export const prepareLoading = () => ({
    type: t.PREPARE_LOADING
});

export const loaded = (source) => ({
    type: t.LOADED,
    payload: source
});