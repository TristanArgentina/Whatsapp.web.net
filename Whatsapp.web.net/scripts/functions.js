async function modificarErrorStack() {
    const originalError = Error;
    // eslint-disable-next-line no-global-assign
    Error = function (message) {
        const error = new originalError(message);
        const originalStack = error.stack;

        if (error.stack.includes('moduleRaid')) {
            error.stack = originalStack + '\n    at https://web.whatsapp.com/vendors~lazy_loaded_low_priority_components.05e98054dbd60f980427.js:2:44';
        }
        return error;
    };
}

function getElementByXpath(path) {
    return document.evaluate(path, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
}

function observeProgress(selectors) {
    var observer = new MutationObserver(function () {
        let progressBar = window.getElementByXpath(
            selectors.PROGRESS
        );
        let progressMessage = window.getElementByXpath(
            selectors.PROGRESS_MESSAGE
        );

        if (progressBar) {
            window.onLoadingScreen(
                progressBar.value,
                progressMessage.innerText
            );
        }
    });

    observer.observe(document, {
        attributes: true,
        childList: true,
        characterData: true,
        subtree: true,
    });
}

function startQRCodeObserver(selectors) {
    var objetoJS = JSON.parse(selectors);
    const qrContainer = document.querySelector(objetoJS.QR_CONTAINER);
    window.qrChanged(qrContainer.dataset.ref);

    const observer = new MutationObserver((muts) => {
        muts.forEach(mut => {
            // Listens to qr token change
            if (mut.type === 'attributes' && mut.attributeName === 'data-ref') {
                window.qrChanged(mut.target.dataset.ref);
            }
            // Listens to retry button, when found, click it
            else if (mut.type === 'childList') {
                const retryButton = document.querySelector(objetoJS.QR_RETRY_BUTTON);
                if (retryButton) retryButton.click();
            }
        });
    });
    observer.observe(qrContainer.parentElement, {
        subtree: true,
        childList: true,
        attributes: true,
        attributeFilter: ['data-ref'],
    });
}

function compareWwebVersions() {
    /**
     * Helper function that compares between two WWeb versions. Its purpose is to help the developer to choose the correct code implementation depending on the comparison value and the WWeb version.
     * @param {string} lOperand The left operand for the WWeb version string to compare with
     * @param {string} operator The comparison operator
     * @param {string} rOperand The right operand for the WWeb version string to compare with
     * @returns {boolean} Boolean value that indicates the result of the comparison
     */
    window.compareWwebVersions = (lOperand, operator, rOperand) => {
        if (!['>', '>=', '<', '<=', '='].includes(operator)) {
            throw new class _ extends Error {
                constructor(m) { super(m); this.name = 'CompareWwebVersionsError'; }
            }('Invalid comparison operator is provided');

        }
        if (typeof lOperand !== 'string' || typeof rOperand !== 'string') {
            throw new class _ extends Error {
                constructor(m) { super(m); this.name = 'CompareWwebVersionsError'; }
            }('A non-string WWeb version type is provided');
        }

        lOperand = lOperand.replace(/-beta$/, '');
        rOperand = rOperand.replace(/-beta$/, '');

        while (lOperand.length != rOperand.length) {
            lOperand.length > rOperand.length
                ? rOperand += '0'
                : lOperand += '0';
        }

        lOperand = parseInt(lOperand.replace('.', ''));
        rOperand = parseInt(rOperand.replace('.', ''));

        return (
            operator == '>' ? lOperand > rOperand :
                operator == '>=' ? lOperand >= rOperand :
                    operator == '<' ? lOperand < rOperand :
                        operator == '<=' ? lOperand <= rOperand :
                            operator == '=' ? lOperand == rOperand :
                                false
        );
    };
}

async function unregisterServiceWorkers() {
    navigator.serviceWorker.getRegistrations()
        .then(registrations => {
            registrations.forEach(registration => {
                registration.unregister();
            });
        });
}

function serializeConnectionAndUser() {
    return {
        ...window.Store.Conn.serialize(),
        wid: window.Store.User.getMeUser()
    };
}

function sendMessageAsyncToChat(chatId, message, options, sendSeen) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    return window.Store.Chat.find(chatWid)
        .then(chat => {
            if (sendSeen) {
                return window.WWebJS.sendSeen(chatId).then(() => chat);
            } else {
                return chat;
            }
        })
        .then(chat => window.WWebJS.sendMessage(chat, message, options, sendSeen))
        .then(msg => window.WWebJS.getMessageModel(msg))
        .catch(err => {
            console.error("Error:", err);
            console.error("Error:", JSON.stringify(err));
            throw err;
        });
}


function getBatteryStatus() {
    const { battery, plugged } = window.Store.Conn;
    return { battery, plugged };
}

async function blockContactById(contactId) {
    const contact = window.Store.Contact.get(contactId);
    return window.Store.BlockContact.blockContact(contact);
}


async function unblockContactById(contactId) {
    const contact = window.Store.Contact.get(contactId);
    return window.Store.BlockContact.unblockContact(contact);
}

async function getStatusContactById(contactId) {
    const wid = window.Store.WidFactory.createWid(contactId);
    return window.Store.StatusUtils.getStatus(wid);
}


async function getProductMetadataById(productId) {
    return window.WWebJS.getProductMetadata(productId);
}

function clearMessagesById(chatId) {
    return window.WWebJS(chatId);
}

function getChatById(chatId) {
    return window.WWebJS.getChat(chatId)
        .then(model => {
            return model;
        })
        .catch((err) => {
            console.log(`Error: ${err}`);
        });
}

function sendClearChatById(chatId) {
    return window.WWebJS.sendClearChat(chatId);
}

function sendDeleteChatById(chatId) {
    window.WWebJS.sendDeleteChat('{Id}');
}

function sendChatStateTypingById(chatId) {
    window.WWebJS.sendChatstate('typing', '{Id}');
}

function sendChatStateRecordingById(chatId) {
    window.WWebJS.sendChatstate('recording', '{Id}');
}
function sendChatStateStopById(chatId) {
    window.WWebJS.sendChatstate('stop', '{Id}');
}

async function getMessagesFromChat(chatId, searchOptions) {
    const msgFilter = (m) => {
        if (m.isNotification) {
            return false; // dont include notification messages
        }
        if (searchOptions && searchOptions.fromMe !== undefined && m.id.fromMe !== searchOptions.fromMe) {
            return false;
        }
        return true;
    };

    const chat = window.Store.Chat.get(chatId);
    let msgs = chat.msgs.getModelsArray().filter(msgFilter);

    if (searchOptions && searchOptions.limit > 0) {
        while (msgs.length < searchOptions.limit) {
            const loadedMessages = await window.Store.ConversationMsgs.loadEarlierMsgs(chat);
            if (!loadedMessages || !loadedMessages.length) break;
            msgs = [...loadedMessages.filter(msgFilter), ...msgs];
        }

        if (msgs.length > searchOptions.limit) {
            msgs.sort((a, b) => (a.t > b.t) ? 1 : -1);
            msgs = msgs.splice(msgs.length - searchOptions.limit);
        }
    }

    return msgs.map(m => window.WWebJS.getMessageModel(m));
}

async function getContactById(contactId) {
    return window.WWebJS.getContact(contactId)
}

async function addParticipantsToGroup(groupId, participantIds, options) {
    const { sleep = [250, 500], autoSendInviteV4 = true, comment = '' } = options;
    const participantData = {};

    !Array.isArray(participantIds) && (participantIds = [participantIds]);
    const groupWid = window.Store.WidFactory.createWid(groupId);
    const group = await window.Store.Chat.find(groupWid);
    const participantWids = participantIds.map((p) => window.Store.WidFactory.createWid(p));

    const errorCodes = {
        default: 'An unknown error occupied while adding a participant',
        isGroupEmpty: 'AddParticipantsError: The participant can\'t be added to an empty group',
        iAmNotAdmin: 'AddParticipantsError: You have no admin rights to add a participant to a group',
        200: 'The participant was added successfully',
        403: 'The participant can be added by sending private invitation only',
        404: 'The phone number is not registered on WhatsApp',
        408: 'You cannot add this participant because they recently left the group',
        409: 'The participant is already a group member',
        417: 'The participant can\'t be added to the community. You can invite them privately to join this group through its invite link',
        419: 'The participant can\'t be added because the group is full'
    };

    await window.Store.GroupMetadata.queryAndUpdate(groupWid);
    const groupMetadata = group.groupMetadata;
    const groupParticipants = groupMetadata?.participants;

    if (!groupParticipants) {
        return errorCodes.isGroupEmpty;
    }

    if (!group.iAmAdmin()) {
        return errorCodes.iAmNotAdmin;
    }

    const _getSleepTime = (sleep) => {
        if (!Array.isArray(sleep) || sleep.length === 2 && sleep[0] === sleep[1]) {
            return sleep;
        }
        if (sleep.length === 1) {
            return sleep[0];
        }
        (sleep[1] - sleep[0]) < 100 && (sleep[0] = sleep[1]) && (sleep[1] += 100);
        return Math.floor(Math.random() * (sleep[1] - sleep[0] + 1)) + sleep[0];
    };

    for (const pWid of participantWids) {
        const pId = pWid;

        participantData[pId] = {
            code: undefined,
            message: undefined,
            isInviteV4Sent: false
        };

        if (groupParticipants.some(p => p.id === pId)) {
            participantData[pId].code = 409;
            participantData[pId].message = errorCodes[409];
            continue;
        }

        if (!(await window.Store.QueryExist(pWid))?.wid) {
            participantData[pId].code = 404;
            participantData[pId].message = errorCodes[404];
            continue;
        }

        const rpcResult =
            await window.WWebJS.getAddParticipantsRpcResult(groupMetadata, groupWid, pWid);
        const { code: rpcResultCode } = rpcResult;

        participantData[pId].code = rpcResultCode;
        participantData[pId].message =
            errorCodes[rpcResultCode] || errorCodes.default;

        if (autoSendInviteV4 && rpcResultCode === 403) {
            let userChat, isInviteV4Sent = false;
            window.Store.ContactCollection.gadd(pWid, { silent: true });

            if (rpcResult.name === 'ParticipantRequestCodeCanBeSent' &&
                (userChat = await window.Store.Chat.find(pWid))) {
                const groupName = group.formattedTitle || group.name;
                const res = await window.Store.GroupInviteV4.sendGroupInviteMessage(
                    userChat,
                    group.id,
                    groupName,
                    rpcResult.inviteV4Code,
                    rpcResult.inviteV4CodeExp,
                    comment,
                    await window.WWebJS.getProfilePicThumbToBase64(groupWid)
                );
                isInviteV4Sent = window.compareWwebVersions(window.Debug.VERSION, '<', '2.2335.6')
                    ? res === 'OK'
                    : res.messageSendResult === 'OK';
            }

            participantData[pId].isInviteV4Sent = isInviteV4Sent;
        }

        sleep &&
            participantWids.length > 1 &&
            participantWids.indexOf(pWid) !== participantWids.length - 1 &&
            (await new Promise((resolve) => setTimeout(resolve, _getSleepTime(sleep))));
    }

    return participantData;
}

function removeParticipants(chatId, participantIds) {
    const chatWid = window.Store.WidFactory.createWid(chatId);

    return window.Store.Chat.find(chatWid)
        .then(chat => {
            const participants = participantIds.map(p => { return chat.groupMetadata.participants.get(p); })
                .filter(p => Boolean(p));

            return window.Store.GroupParticipants.removeParticipants(chat, participants)
                .then(() => ({ status: 200 }));
        })
        .catch(error => {
            console.error("Error:", error);
            throw error;
        });
}

function promoteParticipants(chatId, participantIds) {
    const chatWid = window.Store.WidFactory.createWid(chatId);

    return Promise.all([
        window.Store.Chat.find(chatWid),
        Promise.resolve(participantIds.map(p => ({ id: p })))])
        .then(([chat, participants]) => {
            const validParticipants = participants.filter(p => chat.groupMetadata.participants.has(p.id));
            return window.Store.GroupParticipants.promoteParticipants(chat, validParticipants)
                .then(() => ({ status: 200 }));
        })
        .catch(error => {
            console.error("Error:", error);
            throw error;
        });
}


async function demoteParticipants(chatId, participantIds) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    const chat = await window.Store.Chat.find(chatWid);
    const participants = participantIds.map(p => {
        return chat.groupMetadata.participants.get(p);
    }).filter(p => Boolean(p));
    await window.Store.GroupParticipants.demoteParticipants(chat, participants);
    return { status: 200 };
}

async function setGroupSubject(chatId, subject) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    try {
        await window.Store.GroupUtils.setGroupSubject(chatWid, subject);
        return true;
    } catch (err) {
        if (err.name === 'ServerStatusCodeError') return false;
        throw err;
    }
}

async function setGroupDescription(chatId, description) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    let descId = window.Store.GroupMetadata.get(chatWid).descId;
    let newId = await window.Store.MsgKey.newId();
    try {
        await window.Store.GroupUtils.setGroupDescription(chatWid, description, newId, descId);
        return true;
    } catch (err) {
        if (err.name === 'ServerStatusCodeError') return false;
        throw err;
    }
}

async function setGroupAnnouncement(chatId, adminsOnly) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    try {
        await window.Store.GroupUtils.setGroupProperty(chatWid, 'announcement', adminsOnly ? 1 : 0);
        return true;
    } catch (err) {
        if (err.name === 'ServerStatusCodeError') return false;
        throw err;
    }
}

async function setGroupRestrict(chatId, adminsOnly) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    try {
        await window.Store.GroupUtils.setGroupProperty(chatWid, 'restrict', adminsOnly ? 1 : 0);
        return true;
    } catch (err) {
        if (err.name === 'ServerStatusCodeError') return false;
        throw err;
    }
}

async function deletePicture(chatid) {
    return window.WWebJS.deletePicture(chatid);
}

async function setPicture(chatid, media) {
    return window.WWebJS.setPicture(chatid, media);
}


async function getInviteCode(chatId) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    return window.Store.GroupInvite.queryGroupInviteCode(chatWid);
}

async function revokeInvite(chatId) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    return window.Store.GroupInvite.resetGroupInviteCode(chatWid);
}

async function sendExitGroup(chatId) {
    const chatWid = window.Store.WidFactory.createWid(chatId);
    const chat = await window.Store.Chat.find(chatWid);
    return window.Store.GroupUtils.sendExitGroup(chat);
}

async function getMessageModelById(msgId) {
    const msg = window.Store.Msg.get(msgId);
    if (!msg) return null;
    return window.WWebJS.getMessageModel(msg);
}

async function getQuotedMessageModel(msgId) {
    const msg = window.Store.Msg.get(msgId);
    if (!msg) return null;
    const quotedMsg = window.Store.QuotedMsg.getQuotedMsgObj(msg);
    return window.WWebJS.getMessageModel(quotedMsg);
}

async function joinGroupViaInviteV4(inviteInfo) {
    let { groupId, fromId, inviteCode, inviteCodeExp } = inviteInfo;
    let userWid = window.Store.WidFactory.createWid(fromId);
    return await window.Store.GroupInviteV4.joinGroupViaInviteV4(inviteCode, String(inviteCodeExp), groupId, userWid);
}


async function reactToMessage(messageId, reaction) {
    if (!messageId) { return Promise.resolve(); }
    return window.Store.Msg.get(messageId)
        .then(function (msg) {
            return window.Store.sendReactionToMsg(msg, reaction);
        });
}

async function forwardMessages(msgId, chatId) {
    let msg = window.Store.Msg.get(msgId);
    let chat = window.Store.Chat.get(chatId);

    return new Promise((resolve, reject) => {
        chat.forwardMessages([msg]).then(() => {
            resolve();
        }).catch(error => {
            reject(error);
        });
    });
}

function getMessageMedia(msgId) {
    const msg = window.Store.Msg.get(msgId);
    const handleMediaDownload = (msg) => {
        return window.Store.DownloadManager.downloadAndMaybeDecrypt({
            directPath: msg.directPath,
            encFilehash: msg.encFilehash,
            filehash: msg.filehash,
            mediaKey: msg.mediaKey,
            mediaKeyTimestamp: msg.mediaKeyTimestamp,
            type: msg.type,
            signal: new AbortController().signal
        }).then(decryptedMedia => {
            return window.WWebJS.arrayBufferToBase64Async(decryptedMedia).then(data => {
                return {
                    data,
                    mimetype: msg.mimetype,
                    filename: msg.filename,
                    filesize: msg.filesize
                };
            });
        });
    }


    if (!msg || !msg.mediaData) {
        return Promise.resolve(undefined);
    }
    if (msg.mediaData.mediaStage !== 'RESOLVED') {
        return msg.downloadMedia({
            downloadEvenIfExpensive: true,
            rmrReason: 1
        }).then(() => {
            return handleMediaDownload(msg);
        });
    }

    if (msg.mediaData.mediaStage.includes('ERROR') || msg.mediaData.mediaStage === 'FETCHING') {
        return Promise.resolve(undefined);
    }

    return handleMediaDownload(msg).catch(e => {
        if (e.status && e.status === 404) return undefined;
        throw e;
    });
}

async function retrieveAndConvertMedia(msgId) {
    return new Promise((resolve, reject) => {
        const msg = window.Store.Msg.get(msgId);
        if (!msg || !msg.mediaData) {
            resolve(null);
            return;
        }

        if (msg.mediaData.mediaStage != 'RESOLVED') {
            msg.downloadMedia({
                directPath: msg.directPath,
                fileEncSha256: msg.encFilehash,
                mediaKey: msg.mediaKey,
                fileSha256: msg.filehash,
                downloadEvenIfExpensive: true,
                rmrReason: 1
            }).then(() => {
                proceedWithConversion();
            }).catch((error) => {
                reject(error);
            });
        } else {
            proceedWithConversion();
        }

        function proceedWithConversion() {
            if (msg.mediaData.mediaStage.includes('ERROR') || msg.mediaData.mediaStage === 'FETCHING') {
                resolve(null);
                return;
            }

            window.Store.DownloadManager.downloadAndDecrypt({
                directPath: msg.directPath,
                encFilehash: msg.encFilehash,
                filehash: msg.filehash,
                mediaKey: msg.mediaKey,
                mediaKeyTimestamp: msg.mediaKeyTimestamp,
                type: msg.type,
                signal: (new AbortController).signal
            }).then((decryptedMedia) => {
                window.WWebJS.arrayBufferToBase64Async(decryptedMedia)
                    .then((data) => {
                        resolve({
                            data: data,
                            mimetype: msg.mimetype,
                            filename: msg.filename,
                            filesize: msg.size
                        });
                    }).catch((error) => {
                        reject(error);
                    });
            }).catch((error) => {
                if (error.status && error.status === 404) {
                    resolve(null);
                    return;
                }
                reject(error);
            });
        }
    });
}


async function deleteMessageAsyncWithPermissions(msgId, everyone) {
    let msg = window.Store.Msg.get(msgId);
    let chat = await window.Store.Chat.find(msg.id.remote);

    const canRevoke = window.Store.MsgActionChecks.canSenderRevokeMsg(msg) || window.Store.MsgActionChecks.canAdminRevokeMsg(msg);
    if (everyone && canRevoke) {
        return window.Store.Cmd.sendRevokeMsgs(chat, [msg], { clearMedia: true, type: msg.id.fromMe ? 'Sender' : 'Admin' });
    }

    return window.Store.Cmd.sendDeleteMsgs(chat, [msg], true);
}

async function starMessageIfAllowed(msgId) {
    return new Promise((resolve, reject) => {
        let msg = window.Store.Msg.get(msgId);

        if (!window.Store.MsgActionChecks.canStarMsg(msg)) {
            resolve(null);
            return;
        }

        window.Store.Chat.find(msg.id.remote).then(chat => {
            window.Store.Cmd.sendStarMsgs(chat, [msg], false).then(() => {
                resolve();
            }).catch(error => {
                reject(error);
            });
        }).catch(error => {
            reject(error);
        });
    });
}

async function unstarMessage(msgId) {
    let msg = window.Store.Msg.get(msgId);

    if (window.Store.MsgActionChecks.canStarMsg(msg)) {
        let chat = await window.Store.Chat.find(msg.id.remote);
        return window.Store.Cmd.sendUnstarMsgs(chat, [msg], false);
    }
}

async function pinMessage(msgId, duration) {
    return await window.WWebJS.pinUnpinMsgAction(msgId, 1, duration);
}

async function unpinMessage(msgId) {
    return await window.WWebJS.pinUnpinMsgAction(msgId, 2);
}


async function getMessageInfo(msgId) {
    const msg = window.Store.Msg.get(msgId);
    if (!msg || !msg.id.fromMe) return null;

    return new Promise((resolve) => {
        setTimeout(async () => {
            resolve(await window.Store.getMsgInfo(msg.id));
        }, (Date.now() - msg.t * 1000 < 1250) ? Math.floor(Math.random() * (1200 - 1100 + 1)) + 1100 : 0);
    });
}

async function getOrderDetail(orderId, token, chatId) {
    return await window.WWebJS.getOrderDetail(orderId, token, chatId);
}


async function manageLabelsInChats(labelIds, chatIds) {
    if (['smba', 'smbi'].indexOf(window.Store.Conn.platform) === -1) {
        throw '[LT01] Only Whatsapp business';
    }

    const labels = window.WWebJS.getLabels().filter(e => labelIds.includes(e.id));
    const chats = window.Store.Chat.filter(e => chatIds.includes(e.id));

    let actions = labels.map(label => ({ id: label.id, type: 'add' }));

    chats.forEach(chat => {
        (chat.labels || []).forEach(labelId => {
            if (!actions.some(action => action.id === labelId)) {
                actions.push({ id: labelId, type: 'remove' });
            }
        });
    });

    return window.Store.Label.addOrRemoveLabels(actions, chats);
}

async function getMessageSerialized(msgId) {
    const msg = window.Store.Msg.get(msgId);
    if (!msg) return null;
    return msg.serialize();
}

async function getReactions(msgId) {
    const msgReactions = await window.Store.Reactions.find(msgId);
    if (!msgReactions || !msgReactions.reactions.length) return null;
    return msgReactions.reactions.serialize();
}


async function editMessage(msgId, message, options) {
    let msg = window.Store.Msg.get(msgId);
    if (!msg) return null;

    let catEdit = window.Store.MsgActionChecks.canEditText(msg) || window.Store.MsgActionChecks.canEditCaption(msg);
    if (catEdit) {
        const msgEdit = await window.WWebJS.editMessage(msg, message, options);
        return msgEdit.serialize();
    }
    return null;
}

async function getProfilePic(contactId) {
    return new Promise((resolve, reject) => {
        try {
            const chatWid = window.Store.WidFactory.createWid(contactId);
            window.Store.ProfilePic.profilePicFind(chatWid)
                .then(profilePic => {
                    resolve(profilePic);
                })
                .catch(err => {
                    if (err.name === 'ServerStatusCodeError') {
                        resolve(undefined);
                    } else {
                        reject(err);
                    }
                });
        } catch (err) {
            reject(err);
        }
    });
}

async function getFormattedNumber(numberId) {
    if (!numberId.endsWith('@s.whatsapp.net'))
        numberId = numberId.replace('c.us', 's.whatsapp.net');

    if (!numberId.includes('@s.whatsapp.net'))
        numberId = `${numberId}@s.whatsapp.net`;

    return window.Store.NumberInfo.formattedPhoneNumber(numberId);
}

async function getCountryCode(numberId) {
    return window.Store.NumberInfo.findCC(numberId);
}


async function getCommonGroups(contactId) {
    let contact = window.Store.Contact.get(contactId);
    if (!contact) {
        const wid = window.Store.WidFactory.createUserWid(contactId);
        const chatConstructor = window.Store.Contact.getModelsArray().find(c => !c.isGroup).constructor;
        contact = new chatConstructor({ id: wid });
    }

    if (contact.commonGroups) {
        return contact.commonGroups.serialize();
    }
    const status = await window.Store.findCommonGroups(contact);
    if (status) {
        return contact.commonGroups.serialize();
    }
    return [];
}


async function sendSeen(chatId) {
    return window.WWebJS.sendSeen(chatId);
}

async function archiveChat(chatId) {
    return new Promise((resolve, reject) => {
        window.Store.Chat.get(chatId)
            .then(chat => {
                window.Store.Cmd.archiveChat(chat, true)
                    .then(() => {
                        resolve(true);
                    })
                    .catch(error => {
                        reject(error);
                    });
            })
            .catch(error => {
                reject(error);
            });
    });
}

function unarchiveChat(chatId) {
    return new Promise((resolve, reject) => {
        window.Store.Chat.get(chatId)
            .then(chat => {
                window.Store.Cmd.archiveChat(chat, false)
                    .then(() => {
                        resolve(false);
                    })
                    .catch(error => {
                        reject(error);
                    });
            })
            .catch(error => {
                reject(error);
            });
    });
}


function pinChat(chatId) {
    return new Promise((resolve, reject) => {
        let chat = window.Store.Chat.get(chatId);
        if (chat.pin) {
            resolve(true);
        } else {
            const MAX_PIN_COUNT = 3;
            const chatModels = window.Store.Chat.getModelsArray();
            if (chatModels.length > MAX_PIN_COUNT) {
                let maxPinned = chatModels[MAX_PIN_COUNT - 1].pin;
                if (maxPinned) {
                    resolve(false);
                } else {
                    window.Store.Cmd.pinChat(chat, true)
                        .then(() => {
                            resolve(true);
                        })
                        .catch(error => {
                            reject(error);
                        });
                }
            } else {
                window.Store.Cmd.pinChat(chat, true)
                    .then(() => {
                        resolve(true);
                    })
                    .catch(error => {
                        reject(error);
                    });
            }
        }
    });
}


function unpinChat(chatId) {
    return new Promise((resolve, reject) => {
        let chat = window.Store.Chat.get(chatId);
        if (!chat.pin) {
            resolve(false);
        } else {
            window.Store.Cmd.pinChat(chat, false)
                .then(() => {
                    resolve(false);
                })
                .catch(error => {
                    reject(error);
                });
        }
    });
}

function muteChat(chatId, timestamp) {
    return new Promise((resolve, reject) => {
        window.Store.Chat.get(chatId)
            .then(chat => {
                chat.mute.mute({ expiration: timestamp, sendDevice: true })
                    .then(() => {
                        resolve();
                    })
                    .catch(error => {
                        reject(error);
                    });
            })
            .catch(error => {
                reject(error);
            });
    });
}

function unmuteChat(chatId) {
    return new Promise((resolve, reject) => {
        window.Store.Chat.get(chatId)
            .then(chat => {
                window.Store.Cmd.muteChat(chat, false)
                    .then(() => {
                        resolve();
                    })
                    .catch(error => {
                        reject(error);
                    });
            })
            .catch(error => {
                reject(error);
            });
    });
}

function markChatUnread(chatId) {
    return new Promise((resolve, reject) => {
        window.Store.Chat.get(chatId)
            .then(chat => {
                window.Store.Cmd.markChatUnread(chat, true)
                    .then(() => {
                        resolve();
                    })
                    .catch(error => {
                        reject(error);
                    });
            })
            .catch(error => {
                reject(error);
            });
    });
}

async function getChatLabels(chatId) {
    return window.WWebJS.getChatLabels(chatId);
}



function registerEventListeners() {
    console.log('Agregando eventos a Msg');
    window.Store.Msg.on('change', (msg) => { window.onChangeMessageEvent(window.WWebJS.getMessageModel(msg)); });
    window.Store.Msg.on('change:type', (msg) => { window.onChangeMessageTypeEvent(window.WWebJS.getMessageModel(msg)); });
    window.Store.Msg.on('change:ack', (msg, ack) => { window.onMessageAckEvent(window.WWebJS.getMessageModel(msg), ack); });
    window.Store.Msg.on('change:isUnsentMedia', (msg, unsent) => { if (msg.id.fromMe && !unsent) window.onMessageMediaUploadedEvent(window.WWebJS.getMessageModel(msg)); });
    window.Store.Msg.on('remove', (msg) => { if (msg.isNewMsg) window.onRemoveMessageEvent(window.WWebJS.getMessageModel(msg)); });
    window.Store.Msg.on('change:body change:caption', (msg, newBody, prevBody) => { window.onEditMessageEvent(window.WWebJS.getMessageModel(msg), newBody, prevBody); });
    window.Store.Msg.on('add', (msg) => {
        if (msg.isNewMsg) {
            if (msg.type === 'ciphertext') {
                // defer message event until ciphertext is resolved (type changed)
                msg.once('change:type', (msg) => window.onAddMessageEvent(window.WWebJS.getMessageModel(msg)));
                window.onAddMessageCiphertextEvent(window.WWebJS.getMessageModel(msg));
            } else {
                window.onAddMessageEvent(window.WWebJS.getMessageModel(msg));
            }
        }

    });

    console.log('Agregando eventos a Chat');
    //TODO: missing
    //window.Store.Chat.on('remove', async (chat) => { window.onRemoveChatEvent(await window.WWebJS.getChatModel(chat)); });
    //window.Store.Chat.on('change:archive', async (chat, currState, prevState) => { window.onArchiveChatEvent(await window.WWebJS.getChatModel(chat), currState, prevState); });
    //window.Store.Chat.on('change:unreadCount', (chat) => { window.onChatUnreadCountEvent(chat); });


    //console.log('Agregando eventos a Conn');
    window.Store.Conn.on('change:battery', (state) => { window.onBatteryStateChangedEvent(state); });

    console.log('Agregando eventos a Call');
    window.Store.Call.on('add', (call) => { window.onIncomingCall(call); });

    //TODO: missing
    //console.log('Agregando eventos a AppState');
    //window.Store.AppState.on('change:state', (_AppState, state) => { window.onAppStateChangedEvent(state); });

    console.log('Agregando eventos a createOrUpdateReactions');
    {
        const module = window.Store.createOrUpdateReactionsModule;
        const ogMethod = module.createOrUpdateReactions;
        module.createOrUpdateReactions = ((...args) => {
            window.onReaction(args[0].map(reaction => {
                const msgKey = window.Store.MsgKey.fromString(reaction.msgKey);
                const parentMsgKey = window.Store.MsgKey.fromString(reaction.parentMsgKey);
                const timestamp = reaction.timestamp / 1000;

                return { ...reaction, msgKey, parentMsgKey, timestamp };
            }));

            return ogMethod(...args);
        }).bind(module);
    }
}


function rejectCall(peerJid, id) {
    return window.WWebJS.rejectCall(peerJid, id);
}


async function createGroup(title, participants, options) {
    const { messageTimer = 0, parentGroupId, autoSendInviteV4 = true, comment = '' } = options;
    const participantData = {};
    const participantWids = [];
    const failedParticipants = [];
    let createGroupResult, parentGroupWid;

    const addParticipantResultCodes = {
        default: 'An unknown error occupied while adding a participant',
        200: 'The participant was added successfully',
        403: 'The participant can be added by sending private invitation only',
        404: 'The phone number is not registered on WhatsApp'
    };

    for (const participant of participants) {
        const pWid = window.Store.WidFactory.createWid(participant);
        if ((await window.Store.QueryExist(pWid))?.wid) {
            participantWids.push(pWid);
        } else {
            failedParticipants.push(participant);
        }
    }

    if (parentGroupId) {
        parentGroupWid = window.Store.WidFactory.createWid(parentGroupId);
    }

    try {
        createGroupResult = await window.Store.GroupUtils.createGroup(
            title,
            participantWids,
            messageTimer,
            parentGroupWid
        );
    } catch (err) {
        return 'CreateGroupError: An unknown error occupied while creating a group';
    }

    for (const participant of createGroupResult.participants) {
        let isInviteV4Sent = false;
        const participantId = participant.wid._serialized;
        const statusCode = participant.error ?? 200;

        if (autoSendInviteV4 && statusCode === 403) {
            window.Store.ContactCollection.gadd(participant.wid, { silent: true });
            const addParticipantResult = await window.Store.GroupInviteV4.sendGroupInviteMessage(
                await window.Store.Chat.find(participant.wid),
                createGroupResult.wid._serialized,
                createGroupResult.subject,
                participant.invite_code,
                participant.invite_code_exp,
                comment,
                await window.WWebJS.getProfilePicThumbToBase64(createGroupResult.wid)
            );
            isInviteV4Sent = window.compareWwebVersions(window.Debug.VERSION, '<', '2.2335.6') ?
                addParticipantResult === 'OK' :
                addParticipantResult.messageSendResult === 'OK';
        }

        participantData[participantId] = {
            statusCode: statusCode,
            message: addParticipantResultCodes[statusCode] || addParticipantResultCodes.default,
            isGroupCreator: participant.type === 'superadmin',
            isInviteV4Sent: isInviteV4Sent
        };
    }

    for (const f of failedParticipants) {
        participantData[f] = {
            statusCode: 404,
            message: addParticipantResultCodes[404],
            isGroupCreator: false,
            isInviteV4Sent: false
        };
    }

    return { title: title, gid: createGroupResult.wid, participants: participantData };
}
function getChats() {
    return window.WWebJS.getChats();
}

function getContacts() {
    return window.WWebJS.getContacts();
}

function approveMembershipRequestAction(groupId, options = {}) {
    return this.pupPage.evaluate((groupId, options) => {
        const { requesterIds = null, sleep = [250, 500] } = options;
        return window.WWebJS.membershipRequestAction(groupId, 'Approve', requesterIds, sleep);
    }, groupId, options);
}

function rejectMembershipRequestAction(groupId, options) {
    const { requesterIds = null, sleep = [250, 500] } = options;
    return window.WWebJS.membershipRequestAction(groupId, 'Reject', requesterIds, sleep);
}


function getBlockedContacts() {
    let chatIds = window.Store.Blocklist.getModelsArray().map(a => a.id._serialized);
    return Promise.all(chatIds.map(id => window.WWebJS.getContact(id)));
}
